using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class AdditionalInfoRequest
{
    [JsonPropertyName("ip_address")]
    public string? IpAddress { get; set; }

    [JsonPropertyName("items")]
    public List<AdditionalItemRequest>? Items { get; set; }

    [JsonPropertyName("payer")]
    public AdditionalPayerRequest? Payer { get; set; }

    [JsonPropertyName("shipments")]
    public AdditionalShipmentRequest? Shipments { get; set; }
}

