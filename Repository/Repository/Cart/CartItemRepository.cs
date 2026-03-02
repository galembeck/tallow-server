using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class CartItemRepository : BaseRepository<CartItem>, ICartItemRepository
{
    public CartItemRepository(AppDbContext context) : base(context, context.CartItems) { }

    public async Task<CartItem?> GetItemByProductIdAsync(string cartId, string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(i => i.CartId == cartId && i.ProductId == productId && i.DeletedAt == null)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<List<CartItem>> GetItemsByCartIdAsync(string cartId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Include(i => i.Product)
                .Where(i => i.CartId == cartId && i.DeletedAt == null)
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
