using API.Public.DTOs._Base;
using Domain.Data.Entities;
using Domain.Enumerators;

namespace API.Public.DTOs;

public class ProductResponseDTO : PublicBaseDTO<Product>
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

    public string ImageUrl { get; set; }
    public List<string>? AdditionalImagesUrls { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }



    public ProductResponseDTO() { }

    public ProductResponseDTO(Product o) : base(o)
    {
        if (o == null) return;

        Id = o.Id;
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
        ImageUrl = o.ImageUrl;
        AdditionalImagesUrls = o.AdditionalImagesUrls;
        CreatedAt = o.CreatedAt;
        CreatedBy = o.CreatedBy;
    }

    public static ProductResponseDTO ModelToDTO(Product o) => o == null ? null : new ProductResponseDTO(o);

    public static List<ProductResponseDTO> ModelToDTO(IEnumerable<Product> products) => products.Select(product => new ProductResponseDTO(product)).ToList();
}
