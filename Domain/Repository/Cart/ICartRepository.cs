using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface ICartRepository : IRepository<Cart>
{
    Task<Cart?> GetActiveCartByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Cart?> GetCartWithItemsAsync(string cartId, CancellationToken cancellationToken = default);
}
