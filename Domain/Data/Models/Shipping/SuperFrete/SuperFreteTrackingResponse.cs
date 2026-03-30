using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class SuperFreteTrackingResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; }

    [JsonPropertyName("service_id")]
    public int? ServiceId { get; set; }

    [JsonPropertyName("tracking")]
    public string? Tracking { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("tracking_url")]
    public string? TrackingUrl { get; set; }

    [JsonPropertyName("events")]
    public List<SuperFreteTrackingEvent>? Events { get; set; }
}

public class SuperFreteTrackingEvent
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("location")]
    public string? Location { get; set; }

    [JsonPropertyName("date")]
    public string? Date { get; set; }

    [JsonPropertyName("created_at")]
    public string? CreatedAt { get; set; }
}
