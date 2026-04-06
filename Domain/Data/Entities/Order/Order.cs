using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using Domain.Enumerators;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBOrder")]
public class Order : BaseEntity, IBaseEntity<Order>
{
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public OrderStatus Status { get; set; }

    public decimal SubTotalAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public string ShippingService { get; set; }
    public string ShippingDeliveryTime { get; set; }

    public string ShippingZipcode { get; set; }
    public string ShippingAddress { get; set; }
    public string ShippingNumber { get; set; }
    public string? ShippingComplement { get; set; }
    public string ShippingNeighborhood { get; set; }
    public string ShippingCity { get; set; }
    public string ShippingState { get; set; }

    public string BuyerName { get; set; }
    public string BuyerEmail { get; set; }
    public string BuyerCellphone { get; set; }
    public string BuyerDocument { get; set; }

    public string? TrackingCode { get; set; }
    public string? SuperFreteOrderId { get; set; }
    public string? SuperFreteLabelUrl { get; set; }

    public DateTime? PaymentApprovedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }

    public string? CustomerNotes { get; set; }
    public string? AdminNotes { get; set; }

    public string? CouponCode { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal? DiscountPercentage { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountAmount { get; set; }



    #region .: METHODS :.

    public Order WithoutRelations(Order entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new Order()
        {
            UserId = entity.UserId,
            Status = entity.Status,
            SubTotalAmount = entity.SubTotalAmount,
            ShippingAmount = entity.ShippingAmount,
            TotalAmount = entity.TotalAmount,
            ShippingService = entity.ShippingService,
            ShippingDeliveryTime = entity.ShippingDeliveryTime,
            ShippingZipcode = entity.ShippingZipcode,
            ShippingAddress = entity.ShippingAddress,
            ShippingNumber = entity.ShippingNumber,
            ShippingComplement = entity.ShippingComplement,
            ShippingNeighborhood = entity.ShippingNeighborhood,
            ShippingCity = entity.ShippingCity,
            ShippingState = entity.ShippingState,
            BuyerName = entity.BuyerName,
            BuyerEmail = entity.BuyerEmail,
            BuyerCellphone = entity.BuyerCellphone,
            BuyerDocument = entity.BuyerDocument,
            TrackingCode = entity.TrackingCode,
            SuperFreteOrderId = entity.SuperFreteOrderId,
            SuperFreteLabelUrl = entity.SuperFreteLabelUrl,
            PaymentApprovedAt = entity.PaymentApprovedAt,
            ShippedAt = entity.ShippedAt,
            DeliveredAt = entity.DeliveredAt,
            CancelledAt = entity.CancelledAt,
            CustomerNotes = entity.CustomerNotes,
            AdminNotes = entity.AdminNotes,
            CouponCode = entity.CouponCode,
            DiscountPercentage = entity.DiscountPercentage,
            DiscountAmount = entity.DiscountAmount
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: METHODS :.
}