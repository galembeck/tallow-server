namespace API.Public.DTOs.Shipping;

public class CartShippingQuoteRequestDTO
{
    public string ToZipCode { get; set; }
    public List<CartItemDTO> Items { get; set; } = new();

    public CartShippingQuoteRequestDTO() { }
}

public class CartItemDTO
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }
}
