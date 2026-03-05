using API.Public.DTOs._Base;
using Domain.Data.Entities;
using Domain.Data.Models;

namespace API.Public.DTOs;

public class CreateOrderDTO
{
    public string CartId { get; set; }

    public BuyerInfo BuyerInfo { get; set; }
    public ShippingInfo ShippingInfo { get; set; }
}
