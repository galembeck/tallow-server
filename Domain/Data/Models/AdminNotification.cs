namespace Domain.Data.Models;

public class AdminNotification
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// ORDER_CREATED | PAYMENT_APPROVED | PAYMENT_DECLINED | ORDER_SHIPPED
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// ORDER | PAYMENT | SHIPPING
    /// </summary>
    public string Category { get; set; } = string.Empty;

    public string? OrderId { get; set; }

    public string Message { get; set; } = string.Empty;

    public object? Data { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
