using Domain.Data.Models.Email;

namespace Domain.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string recipientName, string recipientEmail);

    Task SendOrderCreatedEmailAsync(OrderCreatedEmailData data);

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

    Task SendPasswordRecoveryEmailAsync(
        string recipientName,
        string recipientEmail,
        string token,
        DateTime expiresAt);
}