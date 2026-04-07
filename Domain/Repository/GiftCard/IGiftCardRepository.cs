using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IGiftCardRepository : IRepository<GiftCard>
{
    Task<GiftCard?> GetByIdWithPaymentAsync(string id, CancellationToken ct = default);
    Task<List<GiftCard>> GetByUserIdAsync(string userId, CancellationToken ct = default);
    Task<List<GiftCard>> GetAllWithRelationsAsync(CancellationToken ct = default);
}
