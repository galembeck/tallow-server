using Domain.Data.Entities;
using Domain.Data.Models;
using Domain.Enumerators;

namespace Domain.Services;

public interface IOrderService
{
    Task<Order> CreateOrderFromCartAsync(string userId, string cartId, BuyerInfo buyerInfo, ShippingInfo shippingInfo, CancellationToken cancellationToken = default);
    Task<Order?> GetOrderByIdAsync(string orderId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<Order> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus, CancellationToken cancellationToken = default);

    // Admin shipping workflow
    Task<Order> MarkAsPreparingAsync(string orderId, string? adminNotes, CancellationToken cancellationToken = default);
    Task<Order> ShipOrderAsync(string orderId, int serviceCode, float packageWeight, float packageHeight, float packageWidth, float packageLength, string? adminNotes, CancellationToken cancellationToken = default);
    Task<Order> MarkAsDeliveredAsync(string orderId, CancellationToken cancellationToken = default);
    Task<Order> CancelOrderAsync(string orderId, string? adminNotes, CancellationToken cancellationToken = default);
    Task<SuperFreteTrackingResponse> GetOrderTrackingAsync(string orderId, CancellationToken cancellationToken = default);
    Task<(Stream Content, string ContentType)> GetOrderLabelAsync(string orderId, CancellationToken cancellationToken = default);
}