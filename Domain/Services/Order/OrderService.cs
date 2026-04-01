using Domain.Constants;
using Domain.Data.Entities;
using Domain.Data.Models;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Hangfire;

namespace Domain.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IBackgroundJobClient backgroundJobClient) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;

    public async Task<Order> CreateOrderFromCartAsync(
        string userId,
        string cartId,
        BuyerInfo buyerInfo,
        ShippingInfo shippingInfo,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartWithItemsAsync(cartId, cancellationToken);

        if (cart == null || cart.Items.Count == 0)
            throw new BusinessException(BusinessErrorMessage.CART_NOT_FOUND_OR_EMPTY);

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.PENDING,

            SubTotalAmount = cart.TotalAmount,
            ShippingAmount = shippingInfo.ShippingAmount,
            TotalAmount = cart.TotalAmount + shippingInfo.ShippingAmount,

            Items = cart.Items.Select(cartItem => new OrderItem
            {
                Id = Guid.NewGuid().ToString(),
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow,
                CreatedBy = userId,
                UpdatedBy = userId,
                ProductId = cartItem.ProductId,
                ProductName = cartItem.Product.Name,
                ProductImageUrl = cartItem.Product.ImageUrl,
                UnitPrice = cartItem.Product.Price,
                Quantity = cartItem.Quantity
            }).ToList(),

            BuyerName = buyerInfo.Name,
            BuyerEmail = buyerInfo.Email,
            BuyerCellphone = buyerInfo.Cellphone,
            BuyerDocument = buyerInfo.Document,

            ShippingService = shippingInfo.ShippingService,
            ShippingDeliveryTime = shippingInfo.ShippingDeliveryTime,

            ShippingZipcode = shippingInfo.ShippingZipcode,
            ShippingAddress = shippingInfo.ShippingAddress,
            ShippingNumber = shippingInfo.ShippingNumber,
            ShippingComplement = shippingInfo.ShippingComplement,
            ShippingNeighborhood = shippingInfo.ShippingNeighborhood,
            ShippingCity = shippingInfo.ShippingCity,
            ShippingState = shippingInfo.ShippingState
        };

        await _orderRepository.InsertAsync(order);

        _backgroundJobClient.Enqueue<IEmailService>(s =>
            s.SendOrderCreatedEmailAsync(
                order.BuyerName,
                order.BuyerEmail,
                order.Id,
                order.TotalAmount,
                order.ShippingCity,
                order.ShippingState,
                order.Items.Count));

        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(string orderId, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetByIdWithItemsAsync(orderId, cancellationToken);
    }

    public async Task<List<Order>> GetUserOrdersAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<List<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        return await _orderRepository.GetAllWithRelationsAsync(cancellationToken);
    }

    public async Task<Order> UpdateOrderStatusAsync(
        string orderId,
        OrderStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        var updated = await _orderRepository.UpdatePartialAsync(
            new Order { Id = orderId },
            order =>
            {
                order.Status = newStatus;

                if (newStatus == OrderStatus.PAYMENT_APPROVED)
                    order.PaymentApprovedAt = DateTime.UtcNow;
                else if (newStatus == OrderStatus.SHIPPED)
                    order.ShippedAt = DateTime.UtcNow;
                else if (newStatus == OrderStatus.DELIVERED)
                    order.DeliveredAt = DateTime.UtcNow;
                else if (newStatus == OrderStatus.CANCELLED)
                    order.CancelledAt = DateTime.UtcNow;
            });

        if (newStatus == OrderStatus.PROCESSING)
        {
            _backgroundJobClient.Enqueue<IEmailService>(s =>
                s.SendOrderInPreparationEmailAsync(
                    updated.BuyerName,
                    updated.BuyerEmail,
                    updated.Id,
                    updated.ShippingDeliveryTime));
        }
        else if (newStatus == OrderStatus.SHIPPED && !string.IsNullOrWhiteSpace(updated.TrackingCode))
        {
            _backgroundJobClient.Enqueue<IEmailService>(s =>
                s.SendOrderShippedEmailAsync(
                    updated.BuyerName,
                    updated.BuyerEmail,
                    updated.Id,
                    updated.TrackingCode!,
                    updated.ShippingService ?? "Correios"));
        }

        return updated;
    }

    public async Task<Order> UpdateOrderSuperFreteDataAsync(
        string orderId,
        string? superFreteOrderId = null,
        string? trackingCode = null,
        string? labelUrl = null,
        CancellationToken cancellationToken = default)
    {
        return await _orderRepository.UpdatePartialAsync(
            new Order { Id = orderId },
            order =>
            {
                if (superFreteOrderId is not null)
                    order.SuperFreteOrderId = superFreteOrderId;

                if (trackingCode is not null)
                    order.TrackingCode = trackingCode;

                if (labelUrl is not null)
                    order.SuperFreteLabelUrl = labelUrl;
            });
    }
}
