using Domain.Data.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class GiftCardRepository : BaseRepository<GiftCard>, IGiftCardRepository
{
    public GiftCardRepository(AppDbContext context) : base(context, context.GiftCards) { }

    public async Task<GiftCard?> GetByIdWithPaymentAsync(string id, CancellationToken ct = default)
        => await _context.Set<GiftCard>()
            .Include(g => g.Payment)
            .Include(g => g.User)
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<List<GiftCard>> GetByUserIdAsync(string userId, CancellationToken ct = default)
        => await _context.Set<GiftCard>()
            .Include(g => g.Payment)
            .Where(g => g.UserId == userId && g.DeletedAt == null)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);

    public async Task<List<GiftCard>> GetAllWithRelationsAsync(CancellationToken ct = default)
        => await _context.Set<GiftCard>()
            .Include(g => g.User)
            .Include(g => g.Payment)
            .Where(g => g.DeletedAt == null)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync(ct);
}
