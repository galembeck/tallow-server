namespace API.Public.DTOs;

public class CouponValidateResponseDTO
{
    public bool IsValid { get; set; }
    public string? Code { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public string? Message { get; set; }
}
