using Domain.Constants;
using Domain.Data.Entities;
using Domain.Data.Models;
using Domain.Data.Models.Email;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Hangfire;

namespace Domain.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IProductRepository productRepository,
    ICouponRepository couponRepository,
    IBackgroundJobClient backgroundJobClient) : IOrderService
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly ICartRepository _cartRepository = cartRepository;
    private readonly IProductRepository _productRepository = productRepository;
    private readonly ICouponRepository _couponRepository = couponRepository;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;

    private static readonly OrderStatus[] _stockDeductedStatuses =
    [
        OrderStatus.PAYMENT_APPROVED,
        OrderStatus.PREPARING,
        OrderStatus.SHIPPED
    ];

    public async Task<Order> CreateOrderFromCartAsync(
        string userId,
        string cartId,
        BuyerInfo buyerInfo,
        ShippingInfo shippingInfo,
        string? couponCode = null,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetCartWithItemsAsync(cartId, cancellationToken);

        if (cart == null || cart.Items.Count == 0)
            throw new BusinessException(BusinessErrorMessage.CART_NOT_FOUND_OR_EMPTY);

        decimal discountAmount = 0;
        decimal? discountPercentage = null;
        string? appliedCouponCode = null;

        string? couponIdToIncrement = null;

        if (!string.IsNullOrWhiteSpace(couponCode))
        {
            var coupon = await _couponRepository.GetByCodeAsync(couponCode, cancellationToken);
            if (coupon != null && coupon.IsActive &&
                (!coupon.ExpiresAt.HasValue || coupon.ExpiresAt.Value > DateTime.UtcNow))
            {
                discountPercentage = coupon.DiscountPercentage;
                appliedCouponCode = coupon.Code;
                discountAmount = Math.Round(cart.TotalAmount * (coupon.DiscountPercentage / 100m), 2);
                couponIdToIncrement = coupon.Id;
            }
        }

        var order = new Order
        {
            UserId = userId,
            Status = OrderStatus.PENDING,

            SubTotalAmount = cart.TotalAmount,
            ShippingAmount = shippingInfo.ShippingAmount,
            TotalAmount = cart.TotalAmount + shippingInfo.ShippingAmount - discountAmount,

            CouponCode = appliedCouponCode,
            DiscountPercentage = discountPercentage,
            DiscountAmount = discountAmount > 0 ? discountAmount : null,

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

        if (couponIdToIncrement != null)
            await _couponRepository.UpdatePartialAsync(
                new Coupon { Id = couponIdToIncrement },
                c => c.UsageCount += 1);

        var emailData = new OrderCreatedEmailData
        {
            OrderId             = order.Id,
            OrderDate           = order.CreatedAt.UtcDateTime,
            BuyerName           = order.BuyerName,
            BuyerEmail          = order.BuyerEmail,
            SubTotalAmount      = order.SubTotalAmount,
            ShippingAmount      = order.ShippingAmount,
            TotalAmount         = order.TotalAmount,
            ShippingService     = order.ShippingService,
            ShippingDeliveryTime = order.ShippingDeliveryTime,
            ShippingZipcode     = order.ShippingZipcode,
            ShippingAddress     = order.ShippingAddress,
            ShippingNumber      = order.ShippingNumber,
            ShippingComplement  = order.ShippingComplement,
            ShippingNeighborhood = order.ShippingNeighborhood,
            ShippingCity        = order.ShippingCity,
            ShippingState       = order.ShippingState,
            Items               = order.Items.Select(i => new OrderItemEmailData
            {
                ProductId      = i.ProductId,
                ProductName    = i.ProductName,
                ProductImageUrl = i.ProductImageUrl,
                UnitPrice      = i.UnitPrice,
                Quantity       = i.Quantity,
                SubTotal       = i.SubTotal
            }).ToList()
        };

        _backgroundJobClient.Enqueue<IEmailService>(s => s.SendOrderCreatedEmailAsync(emailData));

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
        if (newStatus == OrderStatus.PAYMENT_APPROVED || newStatus == OrderStatus.CANCELLED)
        {
            var currentOrder = await _orderRepository.GetByIdWithItemsAsync(orderId, cancellationToken);
            if (currentOrder != null)
            {
                if (newStatus == OrderStatus.PAYMENT_APPROVED)
                {
                    foreach (var item in currentOrder.Items)
                    {
                        await _productRepository.UpdatePartialAsync(
                            new Product { Id = item.ProductId },
                            p => p.StockAmount = Math.Max(0, p.StockAmount - item.Quantity));
                    }
                }
                else if (newStatus == OrderStatus.CANCELLED && _stockDeductedStatuses.Contains(currentOrder.Status))
                {
                    foreach (var item in currentOrder.Items)
                    {
                        await _productRepository.UpdatePartialAsync(
                            new Product { Id = item.ProductId },
                            p => p.StockAmount += item.Quantity);
                    }
                }
            }
        }

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

        if (newStatus == OrderStatus.PREPARING)
        {
            // Fetch the full order so BuyerName/Email and ShippingDeliveryTime are populated
            var full = await _orderRepository.GetAsync(orderId, cancellationToken);
            if (full != null)
                _backgroundJobClient.Enqueue<IEmailService>(s =>
                    s.SendOrderInPreparationEmailAsync(
                        full.BuyerName,
                        full.BuyerEmail,
                        full.Id,
                        full.ShippingDeliveryTime));
        }
        // Note: the "order shipped" email is NOT triggered here.
        // It is enqueued by OrderController.ShipOrder via TrackingCodeEmailJob,
        // which polls SuperFrete until the tracking code is available before sending.

        return updated;
    }

    public async Task<Order> CancelOrderAsync(
        string orderId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(orderId, cancellationToken)
            ?? throw new BusinessException(BusinessErrorMessage.ORDER_NOT_FOUND);

        if (order.UserId != userId)
            throw new BusinessException(BusinessErrorMessage.NOT_FOUND);

        OrderStatus[] cancellableStatuses =
        [
            OrderStatus.PENDING,
            OrderStatus.PAYMENT_PENDING,
            OrderStatus.PAYMENT_APPROVED,
            OrderStatus.PREPARING
        ];

        if (!cancellableStatuses.Contains(order.Status))
            throw new BusinessException("Pedido não pode ser cancelado pois já foi enviado ou concluído.");

        return await UpdateOrderStatusAsync(orderId, OrderStatus.CANCELLED, cancellationToken);
    }

    public async Task ClearOrderShippingDataAsync(string orderId, CancellationToken cancellationToken = default)
    {
        await _orderRepository.UpdatePartialAsync(
            new Order { Id = orderId },
            order =>
            {
                order.SuperFreteOrderId = null;
                order.TrackingCode = null;
                order.SuperFreteLabelUrl = null;
            });
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
