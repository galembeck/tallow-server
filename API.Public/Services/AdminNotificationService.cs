using API.Public.Hubs;
using Domain.Data.Models;
using Domain.Services;
using Microsoft.AspNetCore.SignalR;

namespace API.Public.Services;

public class AdminNotificationService : IAdminNotificationService
{
    private readonly IHubContext<AdminNotificationHub> _hubContext;
    private readonly ILogger<AdminNotificationService> _logger;

    public AdminNotificationService(
        IHubContext<AdminNotificationHub> hubContext,
        ILogger<AdminNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyOrderCreatedAsync(string orderId, string buyerName, decimal totalAmount, CancellationToken cancellationToken = default)
    {
        var notification = new AdminNotification
        {
            Type     = "ORDER_CREATED",
            Category = "ORDER",
            OrderId  = orderId,
            Message  = $"<strong>PEDIDO RECEBIDO</strong> | Cliente: {buyerName} - R${totalAmount:F2}",
            Data     = new { orderId, buyerName, totalAmount }
        };

        await SendAsync(notification, cancellationToken);
    }

    public async Task NotifyPaymentApprovedAsync(string orderId, string? paymentId, decimal amount, CancellationToken cancellationToken = default)
    {
        var notification = new AdminNotification
        {
            Type     = "PAYMENT_APPROVED",
            Category = "PAYMENT",
            OrderId  = orderId,
            Message  = $"<strong>PAGAMENTO APROVADO</strong> | Pedido: #{orderId[..8].ToUpper()} - R$ {amount:F2}",
            Data     = new { orderId, paymentId, amount }
        };

        await SendAsync(notification, cancellationToken);
    }

    public async Task NotifyPaymentDeclinedAsync(string orderId, string? paymentId, CancellationToken cancellationToken = default)
    {
        var notification = new AdminNotification
        {
            Type     = "PAYMENT_DECLINED",
            Category = "PAYMENT",
            OrderId  = orderId,
            Message  = $"<strong>PAGAMENTO RECUSADO</strong> | Pedido: #{orderId[..8].ToUpper()}",
            Data     = new { orderId, paymentId }
        };

        await SendAsync(notification, cancellationToken);
    }

    public async Task NotifyOrderShippedAsync(string orderId, string trackingCode, CancellationToken cancellationToken = default)
    {
        var notification = new AdminNotification
        {
            Type     = "ORDER_SHIPPED",
            Category = "SHIPPING",
            OrderId  = orderId,
            Message  = $"<strong>PEDIDO ENVIADO</strong> | Pedido: #{orderId[..8].ToUpper()}",
            Data     = new { orderId, trackingCode }
        };

        await SendAsync(notification, cancellationToken);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Broadcasts to ALL connected admins. The frontend filters by category.
    // ──────────────────────────────────────────────────────────────────────────
    private async Task SendAsync(AdminNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Notificação admin [{Type}] → OrderId={OrderId}: {Message}",
            notification.Type, notification.OrderId, notification.Message);

        await _hubContext.Clients.All.SendAsync("notification", notification, cancellationToken);
    }
}
