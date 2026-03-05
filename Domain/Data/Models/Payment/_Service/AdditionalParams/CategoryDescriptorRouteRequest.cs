using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class CategoryDescriptorRouteRequest
{
    [JsonPropertyName("departure")]
    public string? Departure { get; set; }

    [JsonPropertyName("destination")]
    public string? Destination { get; set; }

    [JsonPropertyName("departure_date_time")]
    public DateTime? DepartureDateTime { get; set; }

    [JsonPropertyName("arrival_date_time")]
    public DateTime? ArrivalDateTime { get; set; }

    [JsonPropertyName("company")]
    public string? Company { get; set; }
}
