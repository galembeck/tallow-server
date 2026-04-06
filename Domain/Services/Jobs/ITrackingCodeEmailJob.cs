namespace Domain.Services;

public interface ITrackingCodeEmailJob
{
    /// <summary>
    /// Checks whether the SuperFrete tracking code is available for the given order.
    /// If it is, persists it and sends the "order shipped" email.
    /// Otherwise, reschedules itself (up to <c>MaxAttempts</c>) with a 5-minute delay.
    /// </summary>
    Task SendWhenTrackingAvailableAsync(string orderId, int attempt);
}
