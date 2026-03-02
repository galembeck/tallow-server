namespace API.Public.DTOs;

public class UpdateCartItemDTO
{
    public string ProductId { get; set; }
    public int Quantity { get; set; }

    public UpdateCartItemDTO() { }
}
