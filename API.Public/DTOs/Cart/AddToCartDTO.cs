namespace API.Public.DTOs;

public class AddToCartDTO
{
    public string ProductId { get; set; }
    public int Quantity { get; set; } = 1;

    public AddToCartDTO() { }
}
