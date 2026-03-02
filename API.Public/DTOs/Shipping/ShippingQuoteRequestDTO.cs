using Domain.Data.Models;

namespace API.Public.DTOs;

public class ShippingQuoteRequestDTO
{
    public string ToZipCode { get; set; }
    public string ProductId { get; set; }
    public int Quantity { get; set; } = 1;



    public ShippingQuoteRequestDTO() { }

    public static ShippingQuoteRequest DTOToModel(ShippingQuoteRequestDTO dto, float weight, float height, float width, float length, decimal declaredValue, string fromZipCode)
    {
        return new ShippingQuoteRequest
        {
            FromZipCode = fromZipCode,
            ToZipCode = dto.ToZipCode.Replace("-", "").Replace(".", "").Trim(),
            Weight = (float)weight,
            Height = height,
            Width = width,
            Length = length,
            DeclaredValue = (decimal)declaredValue
        };
    }
}
