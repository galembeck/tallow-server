namespace Domain.Data.Models;

public class ShippingInfo
{
    public string ShippingService { get; set; }
    public string ShippingDeliveryTime { get; set; }
    public decimal ShippingAmount { get; set; }
    public string ShippingZipcode { get; set; }
    public string ShippingAddress { get; set; }
    public string ShippingNumber { get; set; }
    public string? ShippingComplement { get; set; }
    public string ShippingNeighborhood { get; set; }
    public string ShippingCity { get; set; }
    public string ShippingState { get; set; }
}
