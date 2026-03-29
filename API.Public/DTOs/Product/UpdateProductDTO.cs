using API.Public.DTOs._Base;
using Domain.Enumerators;
using ProductEntity = Domain.Data.Entities.Product;

namespace API.Public.DTOs;

public class UpdateProductDTO : PublicBaseDTO<ProductEntity>
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public ProductCategory? Category { get; set; }

    public decimal? Price { get; set; }

    public List<string>? Ingredients { get; set; }

    public int? StockAmount { get; set; }

    public float? Weight { get; set; }
    public float? Height { get; set; }
    public float? Width { get; set; }
    public float? Length { get; set; }

    public IFormFile? Image { get; set; }
    public List<IFormFile>? AdditionalImages { get; set; }



    public UpdateProductDTO() { }

    public static ProductEntity ApplyToModel(UpdateProductDTO dto, ProductEntity existing)
    {
        if (dto.Name != null)        existing.Name        = dto.Name;
        if (dto.Description != null) existing.Description = dto.Description;
        if (dto.Category.HasValue)   existing.Category    = dto.Category.Value;
        if (dto.Price.HasValue)      existing.Price       = dto.Price.Value;
        if (dto.Ingredients != null) existing.Ingredients = dto.Ingredients;
        if (dto.StockAmount.HasValue) existing.StockAmount = dto.StockAmount.Value;
        if (dto.Weight.HasValue)     existing.Weight      = dto.Weight.Value;
        if (dto.Height.HasValue)     existing.Height      = dto.Height.Value;
        if (dto.Width.HasValue)      existing.Width       = dto.Width.Value;
        if (dto.Length.HasValue)     existing.Length      = dto.Length.Value;

        return existing;
    }
}
