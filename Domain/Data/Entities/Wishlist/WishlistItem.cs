using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBWishlistItem")]
public class WishlistItem : BaseEntity, IBaseEntity<WishlistItem>
{
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public string ProductId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }

    #region .: METHODS :.

    public WishlistItem WithoutRelations(WishlistItem entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new WishlistItem
        {
            UserId = entity.UserId,
            ProductId = entity.ProductId
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: METHODS :.
}
