using Domain.Enumerators;
using Domain.Data.Entities;
using Domain.Data.Models;

namespace API.Public.DTOs;

public class OrderResponseDTO
{
    public string Id { get; set; }

    public OrderStatus Status { get; set; }

    public decimal SubTotalAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public List<OrderItemDTO> Items { get; set; }

    public BuyerInfo BuyerInfo { get; set; }
    public ShippingInfo ShippingInfo { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public PaymentSummaryDTO? Payment { get; set; }

    public string? CouponCode { get; set; }
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }


    public static OrderResponseDTO ToDTO(Order order, Payment? payment = null)
    {
        return new OrderResponseDTO
        {
            Id = order.Id,
            Status = order.Status,
            SubTotalAmount = order.SubTotalAmount,
            ShippingAmount = order.ShippingAmount,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,

            Items = order.Items?.Select(i => new OrderItemDTO
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                ProductImageUrl = i.ProductImageUrl,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                SubTotal = i.SubTotal
            }).ToList() ?? new List<OrderItemDTO>(),

            BuyerInfo = new BuyerInfo
            {
                Name = order.BuyerName,
                Email = order.BuyerEmail,
                Cellphone = order.BuyerCellphone,
                Document = order.BuyerDocument
            },

            ShippingInfo = new ShippingInfo
            {
                ShippingService = order.ShippingService,
                ShippingDeliveryTime = order.ShippingDeliveryTime,
                ShippingZipcode = order.ShippingZipcode,
                ShippingAddress = order.ShippingAddress,
                ShippingNumber = order.ShippingNumber,
                ShippingComplement = order.ShippingComplement,
                ShippingNeighborhood = order.ShippingNeighborhood,
                ShippingCity = order.ShippingCity,
                ShippingState = order.ShippingState
            },

            Payment = payment != null ? new PaymentSummaryDTO
            {
                Id = payment.Id,
                Status = payment.Status.ToString(),
                PaymentMethod = payment.PaymentMethod.ToString(),
                TransactionAmount = payment.TransactionAmount
            } : null,

            CouponCode = order.CouponCode,
            DiscountPercentage = order.DiscountPercentage,
            DiscountAmount = order.DiscountAmount
        };
    }
}