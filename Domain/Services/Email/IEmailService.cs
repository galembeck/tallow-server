namespace Domain.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string recipientName, string recipientEmail);

    Task SendOrderCreatedEmailAsync(
        string recipientName,
        string recipientEmail,
        string orderId,
        decimal totalAmount,
        string shippingCity,
        string shippingState,
        int itemsCount);

    Task SendPaymentApprovedEmailAsync(
        string recipientName,
        string recipientEmail,
        string orderId,
        decimal totalAmount,
        string paymentMethod);

    Task SendOrderInPreparationEmailAsync(
        string recipientName,
        string recipientEmail,
        string orderId,
        string? estimatedDelivery);

    Task SendOrderShippedEmailAsync(
        string recipientName,
        string recipientEmail,
        string orderId,
        string trackingCode,
        string shippingService);
}
