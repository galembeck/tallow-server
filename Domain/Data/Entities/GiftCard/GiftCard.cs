using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using Domain.Enumerators;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBGiftCard")]
public class GiftCard : BaseEntity, IBaseEntity<GiftCard>
{
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public GiftCardStatus Status { get; set; } = GiftCardStatus.PENDING;

    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }
    public string? UsedOnOrderId { get; set; }

    // FK to TBPayment (nullable – linked when purchase payment is created)
    public string? PaymentId { get; set; }

    [ForeignKey("PaymentId")]
    public Payment? Payment { get; set; }

    #region .: METHODS :.

    public GiftCard WithoutRelations(GiftCard entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new GiftCard
        {
            UserId        = entity.UserId,
            Amount        = entity.Amount,
            Status        = entity.Status,
            ExpiresAt     = entity.ExpiresAt,
            UsedAt        = entity.UsedAt,
            UsedOnOrderId = entity.UsedOnOrderId,
            PaymentId     = entity.PaymentId
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: METHODS :.
}
