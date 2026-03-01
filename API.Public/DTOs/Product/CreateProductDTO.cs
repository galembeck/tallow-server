using API.Public.DTOs._Base;
using Domain.Enumerators;
using ProductEntity = Domain.Data.Entities.Product;

namespace API.Public.DTOs;

public class CreateProductDTO : PublicBaseDTO<ProductEntity>
{
    public string Name { get; set; }
    public string Description { get; set; }

    public ProductCategory Category { get; set; }

    public decimal Price { get; set; }

    public List<string> Ingredients { get; set; }

    public int StockAmount { get; set; }

    public float Weight { get; set; }
    public float Height { get; set; }
    public float Width { get; set; }
    public float Length { get; set; }

    public IFormFile Image { get; set; }
    public List<IFormFile>? AdditionalImages { get; set; }



    public CreateProductDTO() { }

    public CreateProductDTO(ProductEntity o) : base(o)
    {
        if (o == null) return;

        Name = o.Name;
        Description = o.Description;
        Category = o.Category;
        Price = o.Price;
        Ingredients = o.Ingredients;
        StockAmount = o.StockAmount;
        Weight = o.Weight;
        Height = o.Height;
        Width = o.Width;
        Length = o.Length;
    }

    public static CreateProductDTO ModelToDTO(ProductEntity o) => o == null ? null : new CreateProductDTO(o);

    public static List<CreateProductDTO> ModelToDTO(IEnumerable<ProductEntity> products) => products.Select(product => new CreateProductDTO(product)).ToList();

    public static ProductEntity DTOToModel(CreateProductDTO o)
    {
        if (o == null) return null;

        var model = new ProductEntity()
        {
            Name = o.Name,
            Description = o.Description,
            Category = o.Category,
            Price = o.Price,
            Ingredients = o.Ingredients,
            StockAmount = o.StockAmount,
            Weight = o.Weight,
            Height = o.Height,
            Width = o.Width,
            Length = o.Length
        };

        return o.InitializeInstance(model);
    }
}
