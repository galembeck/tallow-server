using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class AdditionalInfoResponse
{
    [JsonPropertyName("ip_address")]
    public string? IpAddress { get; set; }

    [JsonPropertyName("items")]
    public List<AdditionalItemResponse>? Items { get; set; } 

    [JsonPropertyName("payer")]
    public AdditionalPayerRequest? Payer { get; set; }

    [JsonPropertyName("shipments")]
    public AdditionalShipmentRequest? Shipments { get; set; }
}