using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class AdditionalCategoryDescriptorRequest
{
    [JsonPropertyName("route")]
    public CategoryDescriptorRouteRequest? Route { get; set; }
}
