using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBCart")]
public class Cart : BaseEntity, IBaseEntity<Cart>
{
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public List<CartItem> Items { get; set; } = new List<CartItem>();

    [NotMapped]
    public decimal TotalAmount => Items?.Sum(i => i.SubTotal) ?? 0;

    [NotMapped]
    public int TotalItems => Items?.Sum(i => i.Quantity) ?? 0;



    #region .: METHODS :.

    public Cart WithoutRelations(Cart entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new Cart()
        {
            UserId = entity.UserId,
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: METHODS :.
}
