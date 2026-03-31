using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class SuperFreteCartResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }
}
