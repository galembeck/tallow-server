using Domain.Data.Models;

namespace API.Public.DTOs.Shipping;

public class ShippingQuoteResponseDTO
{
    public string CarrierName { get; set; }
    public string ServiceName { get; set; }
    public decimal Price { get; set; }
    public int DeliveryTime { get; set; }
    public string DeliveryTimeLabel { get; set; }

    public ShippingQuoteResponseDTO() { }

    public ShippingQuoteResponseDTO(ShippingQuoteResponse model)
    {
        if (model == null) return;

        CarrierName = model.CarrierName;
        ServiceName = model.ServiceName;
        Price = model.DeliveryPrice;
        DeliveryTime = model.DeliveryTime;
        DeliveryTimeLabel = $"{model.DeliveryTime} dia{(model.DeliveryTime > 1 ? "s úteis" : " útil")}";
    }

    public static ShippingQuoteResponseDTO ModelToDTO(ShippingQuoteResponse model)
        => model == null ? null : new ShippingQuoteResponseDTO(model);

    public static List<ShippingQuoteResponseDTO> ModelToDTO(IEnumerable<ShippingQuoteResponse> models)
        => models.Select(model => new ShippingQuoteResponseDTO(model)).ToList();
}
