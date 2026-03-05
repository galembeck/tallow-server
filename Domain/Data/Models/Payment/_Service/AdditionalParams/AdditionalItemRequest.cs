using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class AdditionalItemRequest
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
    public int Quantity { get; set; } 

    [JsonPropertyName("unit_price")]
    public decimal UnitPrice { get; set; } 

    // ❌ REMOVIDOS campos que não são usados e podem causar erro
    // [JsonPropertyName("type")]
    // public string? Type { get; set; }

    // [JsonPropertyName("event_date")]
    // public DateTime? EventDate { get; set; }

    // [JsonPropertyName("category_descriptor")]
    // public AdditionalCategoryDescriptorRequest? CategoryDescriptor { get; set; }
}