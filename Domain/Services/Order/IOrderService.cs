using Domain.Data.Entities;
using Domain.Data.Models;
using Domain.Enumerators;

namespace Domain.Services;

public interface IOrderService
{
    Task<Order> CreateOrderFromCartAsync(string userId, string cartId, BuyerInfo buyerInfo, ShippingInfo shippingInfo, string? couponCode = null, CancellationToken cancellationToken = default);
    Task<Order?> GetOrderByIdAsync(string orderId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<Order> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus, CancellationToken cancellationToken = default);
    Task<Order> CancelOrderAsync(string orderId, string userId, CancellationToken cancellationToken = default);
    Task ClearOrderShippingDataAsync(string orderId, CancellationToken cancellationToken = default);
    Task<Order> UpdateOrderSuperFreteDataAsync(string orderId, string? superFreteOrderId = null, string? trackingCode = null, string? labelUrl = null, CancellationToken cancellationToken = default);
}
