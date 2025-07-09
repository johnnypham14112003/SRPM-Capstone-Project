namespace SRPM_Services.BusinessModels.Others;

//for strong-typed config
public class OpenAIOptionModel
{
    public string ApiKey { get; set; } = default!;

    //https://platform.openai.com/docs/pricing#latest-models
    public string ChatModel { get; set; } = "gpt-4o-mini";//default cheapest (2024 data, 0.15$) | gpt-4.1-mini (2025 data, 0.4$)

    //https://platform.openai.com/docs/pricing#embeddings
    public string EmbeddingModel { get; set; } = "text-embedding-3-small"; //text-embedding-3-large | text-embedding-ada-002
}