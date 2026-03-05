using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class IdentificationRequest
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("number")]
    public string Number { get; set; }
}
