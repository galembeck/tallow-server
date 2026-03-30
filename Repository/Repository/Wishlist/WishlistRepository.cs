using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class WishlistRepository : BaseRepository<WishlistItem>, IWishlistRepository
{
    public WishlistRepository(AppDbContext context) : base(context, context.WishlistItems) { }

    public async Task<List<WishlistItem>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Set<WishlistItem>()
                .Include(w => w.Product)
                .Where(w => w.UserId == userId && w.DeletedAt == null)
                .OrderByDescending(w => w.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<WishlistItem?> GetByUserAndProductAsync(string userId, string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Set<WishlistItem>()
                .Include(w => w.Product)
                .Where(w => w.UserId == userId && w.ProductId == productId && w.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
