namespace Domain.Data.Models.Email;

public class OrderCreatedEmailData
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }

    public string BuyerName { get; set; } = string.Empty;
    public string BuyerEmail { get; set; } = string.Empty;

    public List<OrderItemEmailData> Items { get; set; } = [];

    public decimal SubTotalAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public string ShippingService { get; set; } = string.Empty;
    public string? ShippingDeliveryTime { get; set; }

    public string ShippingZipcode { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public string ShippingNumber { get; set; } = string.Empty;
    public string? ShippingComplement { get; set; }
    public string ShippingNeighborhood { get; set; } = string.Empty;
    public string ShippingCity { get; set; } = string.Empty;
    public string ShippingState { get; set; } = string.Empty;
}

public class OrderItemEmailData
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? ProductImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }
}
