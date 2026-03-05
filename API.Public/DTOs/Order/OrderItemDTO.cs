using System.Text.Json.Serialization;

namespace API.Public.DTOs;

public class OrderItemDTO
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string? ProductImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal SubTotal { get; set; }
}

