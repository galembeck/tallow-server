using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Domain.Services;

public class GiftCardService(IGiftCardRepository giftCardRepository) : IGiftCardService
{
    private readonly IGiftCardRepository _repo = giftCardRepository;

    public Task<List<GiftCard>> GetByUserIdAsync(string userId, CancellationToken ct = default)
        => _repo.GetByUserIdAsync(userId, ct);

    public Task<GiftCard?> GetByIdAsync(string id, CancellationToken ct = default)
        => _repo.GetByIdWithPaymentAsync(id, ct);

    public Task<List<GiftCard>> GetAllAsync(CancellationToken ct = default)
        => _repo.GetAllWithRelationsAsync(ct);

    public async Task<GiftCard> CreateAsync(string userId, decimal amount, string? actorId = null, CancellationToken ct = default)
    {
        var gc = new GiftCard
        {
            UserId    = userId,
            Amount    = amount,
            Status    = GiftCardStatus.PENDING,
            ExpiresAt = DateTime.UtcNow.AddMonths(6),
        };

        await _repo.InsertAsync(gc, actorId ?? userId);

        return gc;
    }

    public async Task<GiftCard> LinkPaymentAsync(string id, string paymentId, CancellationToken ct = default)
        => await _repo.UpdatePartialAsync(
            new GiftCard { Id = id },
            g =>
            {
                g.PaymentId = paymentId;
            });

    public async Task<GiftCard> ActivateAsync(string id, CancellationToken ct = default)
        => await _repo.UpdatePartialAsync(
            new GiftCard { Id = id },
            g =>
            {
                g.Status = GiftCardStatus.ACTIVE;
            });

    public async Task<GiftCard> MarkUsedAsync(string id, string orderId, CancellationToken ct = default)
        => await _repo.UpdatePartialAsync(
            new GiftCard { Id = id },
            g =>
            {
                g.Status        = GiftCardStatus.USED;
                g.UsedAt        = DateTime.UtcNow;
                g.UsedOnOrderId = orderId;
            });

    public async Task ExpireEligibleAsync(CancellationToken ct = default)
    {
        var eligible = await _repo
            .GetByExpression(g => g.Status == GiftCardStatus.ACTIVE && g.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync(ct);

        foreach (var gc in eligible)
            await _repo.UpdatePartialAsync(new GiftCard { Id = gc.Id }, g => g.Status = GiftCardStatus.EXPIRED);
    }
}
