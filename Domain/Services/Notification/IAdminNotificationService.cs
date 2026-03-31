namespace Domain.Services;

public interface IAdminNotificationService
{
    Task NotifyOrderCreatedAsync(string orderId, string buyerName, decimal totalAmount, CancellationToken cancellationToken = default);

    Task NotifyPaymentApprovedAsync(string orderId, string? paymentId, decimal amount, CancellationToken cancellationToken = default);

    Task NotifyPaymentDeclinedAsync(string orderId, string? paymentId, CancellationToken cancellationToken = default);

    Task NotifyOrderShippedAsync(string orderId, string trackingCode, CancellationToken cancellationToken = default);
}
