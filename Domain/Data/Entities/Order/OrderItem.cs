using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBOrderItem")]
public class OrderItem : BaseEntity, IBaseEntity<OrderItem>
{
    public string OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    public string ProductId { get; set; }
    [ForeignKey("ProductId")]

    public Product Product { get; set; }

    public string ProductName { get; set; }
    public string? ProductImageUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    [NotMapped]
    public decimal SubTotal => UnitPrice * Quantity;



    #region .: METHODS :.

    public OrderItem WithoutRelations(OrderItem entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new OrderItem()
        {
            OrderId = entity.OrderId,
            ProductId = entity.ProductId,
            ProductName = entity.ProductName,
            ProductImageUrl = entity.ProductImageUrl,
            UnitPrice = entity.UnitPrice,
            Quantity = entity.Quantity
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: METHODS :.
}