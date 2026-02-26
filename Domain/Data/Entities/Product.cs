using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using Domain.Enumerators;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBProduct")]
public class Product : BaseEntity, IBaseEntity<Product>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ProductCategory Category { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    public List<string> Ingredients { get; set; }
    public int StockAmount { get; set; }



    #region .: METHODS :.

    public Product WithoutRelations(Product entity)
    {
        if (entity == null)
            return null!;

        var newEntity = new Product()
        {
            Name = entity.Name,
            Description = entity.Description,
            Category = entity.Category,
            Price = entity.Price,
            Ingredients = entity.Ingredients,
            StockAmount = entity.StockAmount,
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: METHODS :.
}
