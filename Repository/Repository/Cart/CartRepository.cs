using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class CartRepository : BaseRepository<Cart>, ICartRepository
{
    public CartRepository(AppDbContext context) : base(context, context.Carts) { }

    public async Task<Cart?> GetActiveCartByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .Where(c => c.UserId == userId && c.DeletedAt == null)
                .OrderByDescending(c => c.UpdatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        } catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<Cart?> GetCartWithItemsAsync(string cartId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                .Where(c => c.Id == cartId && c.DeletedAt == null)
                .FirstOrDefaultAsync(cancellationToken);
        } catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
