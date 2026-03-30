using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IWishlistRepository : IRepository<WishlistItem>
{
    Task<List<WishlistItem>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<WishlistItem?> GetByUserAndProductAsync(string userId, string productId, CancellationToken cancellationToken = default);
}
