using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class PhoneRequest
{
    [JsonPropertyName("area_code")]
    public string? AreaCode { get; set; }

    [JsonPropertyName("number")]
    public string? Number { get; set; }
}
