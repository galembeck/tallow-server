using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class AdditionalItemResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("picture_url")]
    public string? PictureUrl { get; set; }

    [JsonPropertyName("category_id")]
    public string? CategoryId { get; set; }

    [JsonPropertyName("quantity")]
    public string? Quantity { get; set; }

    [JsonPropertyName("unit_price")]
    public string? UnitPrice { get; set; }
}
