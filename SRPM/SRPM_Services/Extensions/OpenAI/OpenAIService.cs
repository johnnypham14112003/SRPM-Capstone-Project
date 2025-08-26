//https://github.com/openai/openai-dotnet?tab=readme-ov-file
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using OpenAI.Embeddings;
using SRPM_Repositories.Models;
using SRPM_Services.BusinessModels.Others;
using SRPM_Services.BusinessModels.RequestModels;
using SRPM_Services.BusinessModels.ResponseModels;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SRPM_Services.Extensions.OpenAI;

public interface IOpenAIService
{
    Task<string> GetFinalSummaryAsync(string longText, CancellationToken cancellationToken = default);
    int CountToken(string model, string? inputText);
    Task<float[]?> EmbedTextAsync(string inputText, CancellationToken cancellationToken = default);
    Task<RS_AIReviewResult?> ReviewProjectAsync([FromBody] RQ_ProjectContentForAI project, CancellationToken cancellationToken = default);
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
    public int CountToken(string modelName, string? inputText)
    {
        var tokenizer = _tokenizerProvider.GetTokenizer(_openAIOption.EmbeddingModel);
        if (modelName.ToLower().Equals("chatmodel"))
            tokenizer = _tokenizerProvider.GetTokenizer(_openAIOption.ChatModel);

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
            chunks.AddRange(ChunkBySentence(inputText, 7500));
        }

        //List encoded chunks
        var responses = await _embeddingClient.GenerateEmbeddingsAsync(chunks, null, cancellationToken);
        var allVectors = responses.Value.Select(r => r.ToFloats().ToArray()).ToList();

        return AverageVectors(allVectors);
    }

    public async Task<RS_AIReviewResult?> ReviewProjectAsync([FromBody] RQ_ProjectContentForAI project, CancellationToken cancellationToken = default)
    {
        // Build prompt
        var systemRolePrompt = new SystemChatMessage(string.Join(" ",
        [
            "You are a professional evaluator specialized in project assessment.",
            "Your task is to analyze the provided project data and return a structured JSON response with three fields:",
            "1. ReviewerResult (boolean): true if the TotalRate is higher than 50, false otherwise.",
            "2. TotalRate (integer): a score from 0 to 100 that must equal total points you give in the Comment section based on overall quality, clarity, and feasibility and provided data.",
            "3. Comment (string): a detailed explanation in Markdown format, referencing the following evaluation criteria.",
            "The Markdown content must include exactly 7 lines:",
            "- **Purpose and significance of the project** (max 10 points): _your analysis and score_",
            "- **Research methodology** (max 20 points): _your analysis and score_",
            "- **Research content and expected outcomes** (max 40 points): _your analysis and score_",
            "- **Capability of the project leader and research team** (max 20 points): _your analysis and score_",
            "- **Budget justification and cost-effectiveness** (max 10 points): _your analysis and score_",
            "- **Overall assessment**: _a one-sentence summary of the proposal’s overall quality and potential_",
            "- **Detailed conclusion**: _a short about 100 words paragraph synthesizing the evaluation and justifying the final score_",
            "Ensure your conclusion is evidence-grounded, precisely sourced, and maintains warm, clear, and authoritative communication throughout.",
            "Write exclusively in idiomatic English, fully natural and polished for expert audiences.",
            "Respond ONLY with a raw JSON object. Do not include markdown formatting syntax (like triple backticks), code fences, or any extra conversational text."
        ]));
        var projectJson = JsonSerializer.Serialize(project, JsonSerializerOptionInstance.Default);
        var userPrompt = new UserChatMessage("Here is the data:\n" + projectJson);

        //Merge to final prompt
        var messages = new List<ChatMessage> { systemRolePrompt, userPrompt };

        //Send Request
        ChatCompletion response = await _chatClient.CompleteChatAsync(messages, _chatCompletionOptions, cancellationToken);

        var responseText = response.Content[0].Text;
        try
        {
            return JsonSerializer.Deserialize<RS_AIReviewResult>(responseText);
        }
        catch (JsonException ex)
        {
            Console.WriteLine("Failed to parse AI response: ", ex);
            throw;
        }
    }

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
                Similarity = similarity
            });
        }

        return results.OrderByDescending(r => r.Similarity).ToList();
    }

    public async Task<string> GetFinalSummaryAsync(string longText, CancellationToken cancellationToken = default)
    {
        // Divide text into chunks
        var chunks = ChunkBySentence(longText, 6500);

        // Summary each chunnk
        var summaries = new List<string>();
        foreach (var chunk in chunks)
        {
            var summary = await SummarizeChunkAsync(chunk, cancellationToken);
            summaries.Add(summary);
        }

        // Finallize all chunks summaries
        var finalSummary = await SynthesizeSummariesAsync(string.Join("\n", summaries), cancellationToken);
        return finalSummary;
    }

    //=====================================================================
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

    public List<string> ChunkBySentence(string inputText, int maxTokens)
    {
        var tokenizer = _tokenizerProvider.GetTokenizer(_openAIOption.ChatModel);
        var sentences = Regex.Split(inputText, @"(?<=[.!?])\s+");//Split by the symbol
        var chunks = new List<string>();
        var currentChunk = new List<int>();

        foreach (var sentence in sentences)
        {
            var sentenceTokens = tokenizer.EncodeToIds(sentence);
            if (currentChunk.Count + sentenceTokens.Count > maxTokens)
            {
                chunks.Add(tokenizer.Decode(currentChunk));
                currentChunk.Clear();
            }

            currentChunk.AddRange(sentenceTokens);
        }

        if (currentChunk.Count > 0)
            chunks.Add(tokenizer.Decode(currentChunk));

        return chunks;
    }

    // Prompt summarize for 1 chunk
    private async Task<string> SummarizeChunkAsync(string chunk, CancellationToken ct)
    {
        // Prompt
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a professional evaluator reviewing a project proposal. " +
                "Your role is to analyze each section and provide a detailed summary and evaluation based on specific criteria."),
            
            new UserChatMessage($"Please review the following section of the proposal:\n\nCHUNK START\n{chunk}\nCHUNK END\n\n" +
                "Your response must include:\n\n" +
                "1) A concise summary of the section (2–3 sentences)\n\n" +
                "2) Evaluation across the following criteria (if applicable in this chunk):\n" +
                "   - Purpose and significance of the project (max 10 points)\n" +
                "   - Research methodology (max 20 points)\n" +
                "   - Research content and expected outcomes (max 40 points)\n" +
                "   - Capability of the project leader and research team (max 20 points)\n" +
                "   - Budget justification and cost-effectiveness (max 10 points)\n\n" +
                "For each relevant criterion, provide:\n" +
                "   a) A short paragraph of analysis\n" +
                "   b) A score from 0 to the category’s maximum\n\n" +
                "3) Comments on feasibility (technical, logistical, and practical)\n" +
                "4) Comments on clarity (structure, language, and coherence)\n" +
                "5) Comments on creativity (originality, innovation, and impact)\n\n" +
                "Use clear, professional, and evidence-based language. Your tone should be warm, analytical, and suitable for expert audiences."
    )
        };

        ChatCompletion response = await _chatClient.CompleteChatAsync(messages, _chatCompletionOptions, ct);
        return response.Content[0].Text.Trim();
    }

    // Merge summaries -> Prompt Summary
    private async Task<string> SynthesizeSummariesAsync(string summaries, CancellationToken ct)
    {
        var combined = string.Join("\n\n---\n\n", summaries.Select((s, i) => $"Chunk#{i + 1} Summary:\n{s}"));
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a senior evaluator tasked with synthesizing multiple section reviews into a final, " +
                "detailed assessment of a research project proposal. Your evaluation will be used to score the project across specific criteria."),
            new UserChatMessage($"Based on the following chunk reviews, provide a comprehensive evaluation:\n\n{combined}\n\n" +
               "Your response should include:\n" +
               "1) One-paragraph overall summary of the proposal.\n\n" +
                "2) Detailed analysis for each of the following categories:\n" +
                "   - Purpose and significance of the project (max 10 points)\n" +
                "   - Research methodology (max 20 points)\n" +
                "   - Research content and expected outcomes (max 40 points)\n" +
                "   - Capability of the project leader and research team (max 20 points)\n" +
                "   - Budget justification and cost-effectiveness (max 10 points)\n\n" +
                "For each category, provide:\n" +
                "   a) A short paragraph of analysis\n" +
                "   b) A score from 0 to the category’s maximum\n\n" +
                "3) Five bullet-point strengths of the proposal\n" +
                "4) Five bullet-point weaknesses of the proposal\n" +
                "5) Final total score out of 100, with a brief rationale\n\n" +
                "Use clear, professional, and evidence-based language. Your tone should be warm, authoritative, and suitable for expert audiences.")
        };

        ChatCompletion response = await _chatClient.CompleteChatAsync(messages, _chatCompletionOptions, ct);
        return response.Content[0].Text.Trim();
    }
}