using Domain.Data.Entities;
using Domain.Enumerators;

namespace API.Public.DTOs;

/// <summary>
/// Lightweight DTO for the admin orders list/table.
/// Navigate to the detail screen for full buyer and shipping information.
/// </summary>
public class OrderAdminSummaryDTO
{
    public string Id { get; set; }
    public string Status { get; set; }

    public string BuyerName { get; set; }
    public string BuyerEmail { get; set; }

    public decimal SubTotalAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public int ItemsCount { get; set; }

    public string? ShippingCity { get; set; }
    public string? ShippingState { get; set; }

    public DateTimeOffset DateCreated { get; set; }
    public DateTime? PaymentApprovedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }



    public static OrderAdminSummaryDTO ToDTO(Order order) => new()
    {
        Id = order.Id,
        Status = order.Status.ToString(),

        BuyerName = order.BuyerName,
        BuyerEmail = order.BuyerEmail,

        SubTotalAmount = order.SubTotalAmount,
        ShippingAmount = order.ShippingAmount,
        TotalAmount = order.TotalAmount,

        ItemsCount = order.Items?.Count ?? 0,

        ShippingCity = order.ShippingCity,
        ShippingState = order.ShippingState,

        DateCreated = order.CreatedAt,
        PaymentApprovedAt = order.PaymentApprovedAt,
        ShippedAt = order.ShippedAt,
        DeliveredAt = order.DeliveredAt,
        CancelledAt = order.CancelledAt,
    };

    public static List<OrderAdminSummaryDTO> ToDTO(IEnumerable<Order> orders)
        => orders.Select(ToDTO).ToList();
}
