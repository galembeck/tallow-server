using Domain.Data.Entities;

namespace API.Public.DTOs;

public class CouponResponseDTO
{
    public string Id { get; set; }
    public string Code { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public static CouponResponseDTO ToDTO(Coupon coupon) => new CouponResponseDTO
    {
        Id = coupon.Id,
        Code = coupon.Code,
        DiscountPercentage = coupon.DiscountPercentage,
        IsActive = coupon.IsActive,
        CreatedAt = coupon.CreatedAt
    };
}
