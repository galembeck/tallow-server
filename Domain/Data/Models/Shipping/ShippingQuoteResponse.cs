namespace Domain.Data.Models;

public class ShippingQuoteResponse
{
    public string CarrierName { get; set; }
    public string ServiceName { get; set; }

    public decimal DeliveryPrice { get; set; }
    public int DeliveryTime { get; set; }

    public string CarrierCode { get; set; }
    public string ServiceCode { get; set; }

    public string Error { get; set; }
}
