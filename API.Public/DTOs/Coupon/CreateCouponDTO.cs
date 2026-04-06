namespace API.Public.DTOs;

public class CreateCouponDTO
{
    public string Code { get; set; }
    public decimal DiscountPercentage { get; set; }
}
