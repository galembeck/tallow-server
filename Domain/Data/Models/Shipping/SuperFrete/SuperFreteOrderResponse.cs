using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class SuperFreteOrderResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("protocol")]
    public string? Protocol { get; set; }

    [JsonPropertyName("service_id")]
    public int? ServiceId { get; set; }

    [JsonPropertyName("price")]
    public string? Price { get; set; }

    [JsonPropertyName("print")]
    public string? Print { get; set; }

    [JsonPropertyName("tracking")]
    public string? Tracking { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("error")]
    public string? Error { get; set; }
}

/// <summary>
/// Internal model returned by CreateShipmentAsync — the combined result of cart creation + payment.
/// </summary>
public class ShipmentResult
{
    public string SuperFreteOrderId { get; set; }
    public string? TrackingCode { get; set; }
    public string? LabelUrl { get; set; }
    public string? TrackingUrl { get; set; }
}
