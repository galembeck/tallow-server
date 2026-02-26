using System.Text.Json;

namespace Domain.Data.Models.Utils;

public class ValidationErrorModel
{
    public string Property { get; set; }
    public string Message { get; set; }

    public string ToJson()
    {
        JsonSerializerOptions options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        return JsonSerializer.Serialize(this, options);
    }
}