using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface ICartItemRepository : IRepository<CartItem>
{
    Task<CartItem?> GetItemByProductIdAsync(string cartId, string productId, CancellationToken cancellationToken = default);
    Task<List<CartItem>> GetItemsByCartIdAsync(string cartId, CancellationToken cancellationToken = default);
}
