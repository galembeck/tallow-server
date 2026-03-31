using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class SuperFretePrintResponse
{
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
