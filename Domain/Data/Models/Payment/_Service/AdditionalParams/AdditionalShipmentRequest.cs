using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class AdditionalShipmentRequest
{
    [JsonPropertyName("receiver_address")]
    public ReceiverAddressRequest? ReceiverAddress { get; set; }

    //[JsonPropertyName("width")]
    //public decimal Width { get; set; }

    //[JsonPropertyName("height")]
    //public decimal Height { get; set; }

    [JsonPropertyName("express_shipment")]
    public bool? ExpressShipment { get; set; }

    [JsonPropertyName("pick_up_on_seller")]
    public bool? PickUpOnSeller { get; set; }
}
