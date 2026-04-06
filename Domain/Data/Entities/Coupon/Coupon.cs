using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBCoupon")]
public class Coupon : BaseEntity, IBaseEntity<Coupon>
{
    public string Code { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal DiscountPercentage { get; set; }

    public bool IsActive { get; set; } = true;

    public int UsageCount { get; set; } = 0;

    public DateTime? ExpiresAt { get; set; }



    #region .: METHODS :.

    public Coupon WithoutRelations(Coupon entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new Coupon()
        {
            Code = entity.Code,
            DiscountPercentage = entity.DiscountPercentage,
            IsActive = entity.IsActive,
            UsageCount = entity.UsageCount,
            ExpiresAt = entity.ExpiresAt
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: METHODS :.
}
