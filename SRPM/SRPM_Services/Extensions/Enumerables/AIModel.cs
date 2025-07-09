using System.Runtime.Serialization;

namespace SRPM_Services.Extensions.Enumerables;

public enum AIModel
{
    [EnumMember(Value = "gpt-4o-mini")]
    Gpt4oMini,

    [EnumMember(Value = "gpt-4.1-mini")]
    Gpt41Mini,

    [EnumMember(Value = "text-embedding-3-small")]
    TextEmbedding3Small,

    [EnumMember(Value = "text-embedding-3-large")]
    TextEmbedding3Large,
    
    [EnumMember(Value = "text-embedding-ada-002")]
    TextEmbeddingAda002
}