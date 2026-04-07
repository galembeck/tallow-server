using Domain.Data.Entities;

namespace Domain.Services;

public interface IGiftCardService
{
    Task<List<GiftCard>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<GiftCard?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<List<GiftCard>> GetAllAsync(CancellationToken ct = default);
    Task<GiftCard> CreateAsync(string userId, decimal amount, string? actorId = null, CancellationToken ct = default);
    Task<GiftCard> LinkPaymentAsync(string id, string paymentId, CancellationToken ct = default);
    Task<GiftCard> ActivateAsync(string id, CancellationToken ct = default);
    Task<GiftCard> MarkUsedAsync(string id, string orderId, CancellationToken ct = default);
    Task ExpireEligibleAsync(CancellationToken ct = default);
}
