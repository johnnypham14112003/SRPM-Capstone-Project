using System.Text.Json;
using System.Text.Json.Serialization;

namespace SRPM_Services.Extensions;

//Static Instance for cache
public class JsonSerializerOptionInstance
{
    public static readonly JsonSerializerOptions Default = new JsonSerializerOptions
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}