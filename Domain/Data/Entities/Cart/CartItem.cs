using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBCartItem")]
public class CartItem : BaseEntity, IBaseEntity<CartItem>
{
    public string CartId { get; set; }

    [ForeignKey("CartId")]
    public Cart Cart { get; set; }

    public string ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal SubTotal => Quantity * UnitPrice;



    #region .: METHODS :.

    public CartItem WithoutRelations(CartItem entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new CartItem()
        {
            CartId = entity.CartId,
            ProductId = entity.ProductId,
            Quantity = entity.Quantity,
            UnitPrice = entity.UnitPrice,
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: METHODS :.
}
