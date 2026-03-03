namespace Domain.Data.Models;

public class CartShippingQuoteRequest
{
    public string FromZipCode { get; set; }
    public string ToZipCode { get; set; }
    public List<CartProductItem> Products { get; set; } = new();
}

public class CartProductItem
{
    public int Quantity { get; set; }
    public float Weight { get; set; }
    public float Height { get; set; }
    public float Width { get; set; }
    public float Length { get; set; }
    public decimal DeclaredValue { get; set; }
}
