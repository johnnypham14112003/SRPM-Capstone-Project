//https://learn.microsoft.com/en-us/dotnet/api/microsoft.ml.tokenizers.tiktokentokenizer?view=ml-dotnet-preview
using Microsoft.ML.Tokenizers;
using SRPM_Services.BusinessModels.Others;

namespace SRPM_Services.Extensions.OpenAI;

public interface ITokenizerProvider
{
    TiktokenTokenizer GetTokenizer(string model);
}

public class TokenizerProvider : ITokenizerProvider
{
    private readonly OpenAIOptionModel _modelConfigured;
    private readonly Dictionary<string, TiktokenTokenizer> _tokenizers = new();

    public TokenizerProvider(OpenAIOptionModel modelConfigured)
    {
        _modelConfigured = modelConfigured;

        _tokenizers = new()
        {
            [_modelConfigured.ChatModel] = TiktokenTokenizer.CreateForModel(_modelConfigured.ChatModel),
            [_modelConfigured.EmbeddingModel] = TiktokenTokenizer.CreateForModel(_modelConfigured.EmbeddingModel)
        };
        //_tokenizers["text-embedding-3-small"] = TiktokenTokenizer.CreateForModel("text-embedding-3-small");
        //_tokenizers["text-embedding-3-large"] = TiktokenTokenizer.CreateForModel("text-embedding-3-large");
        //_tokenizers["text-embedding-ada-002"] = TiktokenTokenizer.CreateForModel("text-embedding-ada-002");
        //_tokenizers["gpt-4o-mini"] = TiktokenTokenizer.CreateForModel("gpt-4o-mini");
        //_tokenizers["gpt-4.1-mini"] = TiktokenTokenizer.CreateForModel("gpt-4.1-mini");
    }

    //=============================================================================
    public TiktokenTokenizer GetTokenizer(string model)
    {
        return _tokenizers.TryGetValue(model, out var tokenizer)
            ? tokenizer
            : throw new KeyNotFoundException($"Not found tokenizer instance for model: {model}");
    }
}