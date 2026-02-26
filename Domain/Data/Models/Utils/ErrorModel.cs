using System.Text.Json;

namespace Domain.Data.Models.Utils;

public class ErrorModel
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public int? Status { get; set; }
    public ValidationErrorModel Error { get; set; } = new ValidationErrorModel();

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
