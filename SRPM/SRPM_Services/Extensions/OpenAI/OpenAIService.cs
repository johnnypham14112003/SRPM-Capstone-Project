//https://github.com/openai/openai-dotnet?tab=readme-ov-file
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using OpenAI.Embeddings;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using System.Text.Json;

namespace SRPM_Services.Extensions.OpenAI;

public interface IOpenAIService
{
    int CountToken(string model, string? inputText);
    Task<float[]?> EmbedTextAsync(string inputText, CancellationToken cancellationToken = default);
    Task<string> ReviewProjectAsync([FromBody] RQ_ProjectContentForAI project, CancellationToken cancellationToken = default);
    //Task<Dictionary<string, double>> CompareWithSourceAsync(string inputText, IEnumerable<string> inputSource, CancellationToken cancellationToken = default);
    Task<List<RS_ProjectSimilarityResult>> CompareWithSourceAsync(
        float[] inputVector, IEnumerable<RS_ProjectSimilarityResult> projectSource, CancellationToken cancellationToken = default);
}

public class OpenAIService : IOpenAIService
{
    private readonly ITokenizerProvider _tokenizerProvider;
    private readonly ChatClient _chatClient;
    private readonly ChatCompletionOptions _chatCompletionOptions;
    private readonly EmbeddingClient _embeddingClient;
    private readonly OpenAIOptionModel _openAIOption;

    public OpenAIService(
        ITokenizerProvider tokenizerProvider,
        ChatClient chatClient,
        ChatCompletionOptions chatCompletionOptions,
        EmbeddingClient embeddingClient,
        OpenAIOptionModel openAIOption)
    {
        _tokenizerProvider = tokenizerProvider;
        _chatClient = chatClient;
        _embeddingClient = embeddingClient;
        _chatCompletionOptions = chatCompletionOptions;
        _openAIOption = openAIOption;
    }

    //=============================================================================
    public int CountToken(string model, string? inputText)
    {
        var tokenizer = _tokenizerProvider.GetTokenizer(model);
        return string.IsNullOrWhiteSpace(inputText) ? 0 : tokenizer.CountTokens(inputText);
    }

    //Convert human language into vector (machine language)
    public async Task<float[]?> EmbedTextAsync(string inputText, CancellationToken cancellationToken = default)
    {
        var modelName = _openAIOption.EmbeddingModel;
        int maxTokens = 8100; //https://platform.openai.com/docs/guides/embeddings
        var tokenizer = _tokenizerProvider.GetTokenizer(modelName);
        var tokenIds = tokenizer.EncodeToIds(inputText);

        var chunks = new List<string>();
        if (tokenIds.Count <= maxTokens)
        {
            chunks.Add(inputText);
        }
        else
        {
            //Hanlde if text to long => divide into chunks
            for (int i = 0; i < tokenIds.Count; i += maxTokens)
            {
                var slice = tokenIds.Skip(i).Take(maxTokens).ToList();
                chunks.Add(tokenizer.Decode(slice));
            }
        }

        //List encoded chunks
        var responses = await _embeddingClient.GenerateEmbeddingsAsync(chunks, null, cancellationToken);
        var allVectors = responses.Value.Select(r => r.ToFloats().ToArray()).ToList();

        return AverageVectors(allVectors);
    }

    public async Task<string> ReviewProjectAsync([FromBody] RQ_ProjectContentForAI project, CancellationToken cancellationToken = default)
    {
        // Build prompt
        var systemRolePrompt = new SystemChatMessage(string.Join(" ",
        [
            "You are a world-class, adaptive Smart Examiner and analytical expert, combining deep expertise in critical evaluation, meticulous source verification, and comprehensive comparative analysis across digital landscapes.",
            "Maintain warm, clear, and authoritative communication throughout; write exclusively in idiomatic English, fully natural and polished for expert audiences.",
            "Ensuring every conclusion is evidence-grounded, precisely sourced."
        ]));
        var projectJson = JsonSerializer.Serialize(project, JsonSerializerOptionInstance.Default);
        var userPrompt = new UserChatMessage("Here is the data:\n" + projectJson);

        //Merge to final prompt
        var messages = new List<ChatMessage> { systemRolePrompt, userPrompt };

        //Send Request
        ChatCompletion response = await _chatClient.CompleteChatAsync(messages, _chatCompletionOptions, cancellationToken);

        return response.Content[0].Text;
    }

    //public async Task<Dictionary<string, double>> CompareWithSourceAsync(
    //    string inputText, IEnumerable<string> inputSource, CancellationToken cancellationToken = default)
    //{
    //    //Encode Input Text
    //    var embInput = await EmbedTextAsync(inputText, cancellationToken);
    //    var results = new Dictionary<string, double>();

    //    //Encode List Source
    //    foreach (var input in inputSource)
    //    {
    //        var embDoc = await EmbedTextAsync(input, cancellationToken);

    //        //Check Plagiarism
    //        results[input] = CosineSimilarity(embInput, embDoc);
    //    }

    //    return results;
    //}

    public async Task<List<RS_ProjectSimilarityResult>> CompareWithSourceAsync(
        float[] inputVector, IEnumerable<RS_ProjectSimilarityResult> projectSource, CancellationToken cancellationToken = default)
    {
        var results = new List<RS_ProjectSimilarityResult>();

        foreach (var project in projectSource)
        {
            float[]? vector;
            //Skip null
            if (string.IsNullOrWhiteSpace(project.EncodedDescription) && string.IsNullOrWhiteSpace(project.Description))
                continue;

            if (string.IsNullOrWhiteSpace(project.EncodedDescription))
            { vector = await EmbedTextAsync(project.Description!, cancellationToken); }
            else
            {
                // Parse EncodedDescription string to float[]
                vector = JsonSerializer.Deserialize<float[]>(project.EncodedDescription);
            }
            //Skip if still null
            if (vector is null || vector.Length == 0)
                continue;

            //compare
            var similarity = CosineSimilarity(inputVector, vector);

            //Add to list
            results.Add(new RS_ProjectSimilarityResult
            {
                Id = project.Id,
                EnglishTitle = project.EnglishTitle ?? "",
                Description = project.Description,
                EncodedDescription = project.EncodedDescription,
                OnlineUrl = project.OnlineUrl,
                Similarity = similarity
            });
        }

        return results.OrderByDescending(r => r.Similarity).ToList();
    }

    //Calculate the difference
    private static double CosineSimilarity(float[] a, float[] b)
    {
        double dot = 0, da = 0, db = 0;
        for (int i = 0; i < a.Length; i++)
        {
            dot += a[i] * b[i];
            da += a[i] * a[i];
            db += b[i] * b[i];
        }
        return dot / (Math.Sqrt(da) * Math.Sqrt(db));
    }

    //Calculate summary of many vectors
    private static float[]? AverageVectors(List<float[]> vectors)
    {
        if (vectors.Count == 0) return Array.Empty<float>();

        int dim = vectors[0].Length;
        var avg = new float[dim];
        foreach (var vec in vectors)
            for (int i = 0; i < dim; i++)
                avg[i] += vec[i];

        for (int i = 0; i < dim; i++)
            avg[i] /= vectors.Count;
        return avg;
    }
}