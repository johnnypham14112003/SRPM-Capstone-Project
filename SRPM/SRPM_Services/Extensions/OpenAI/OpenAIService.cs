//https://github.com/openai/openai-dotnet?tab=readme-ov-file
using OpenAI;
using OpenAI.Embeddings;
using SRPM_Services.BusinessModels.Others;

namespace SRPM_Services.Extensions.OpenAI;

public interface IOpenAIService
{
    int CountToken(string model, string? inputText);
    Task<Dictionary<string, double>> CompareWithSourceAsync(
        string inputText, IEnumerable<string> inputSource, CancellationToken cancellationToken = default);
}

public class OpenAIService : IOpenAIService
{
    private readonly ITokenizerProvider _tokenizerProvider;
    private readonly OpenAIClient _openAIClient;
    private readonly EmbeddingClient _embeddingClient;
    private readonly OpenAIOptionModel _openAIOption;

    public OpenAIService(
        ITokenizerProvider tokenizerProvider,
        OpenAIClient openAIClient,
        EmbeddingClient embeddingClient,
        OpenAIOptionModel openAIOption)
    {
        _tokenizerProvider = tokenizerProvider;
        _openAIClient = openAIClient;
        _embeddingClient = embeddingClient;
        _openAIOption = openAIOption;
    }

    //=============================================================================
    public int CountToken(string model, string? inputText)
    {
        var tokenizer = _tokenizerProvider.GetTokenizer(model);
        return string.IsNullOrWhiteSpace(inputText) ? 0 : tokenizer.CountTokens(inputText);
    }

    //Convert human language into vector (machine language)
    public async Task<float[]> EmbedTextAsync(string inputText, CancellationToken cancellationToken = default)
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


    public async Task<Dictionary<string, double>> CompareWithSourceAsync(
        string inputText, IEnumerable<string> inputSource, CancellationToken cancellationToken = default)
    {
        //Encode Input Text
        var embInput = await EmbedTextAsync(inputText, cancellationToken);
        var results = new Dictionary<string, double>();

        //Encode 
        foreach (var input in inputSource)
        {
            var embDoc = await EmbedTextAsync(input, cancellationToken);
            results[input] = CosineSimilarity(embInput, embDoc);
        }

        return results;
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
    private static float[] AverageVectors(List<float[]> vectors)
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