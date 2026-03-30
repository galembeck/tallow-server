using Domain.Data.Entities;

namespace Domain.Services;

public interface IWishlistService
{
    Task<List<WishlistItem>> GetUserWishlistAsync(string userId, CancellationToken cancellationToken = default);
    Task<WishlistItem> AddToWishlistAsync(string userId, string productId, CancellationToken cancellationToken = default);
    Task RemoveFromWishlistAsync(string userId, string productId, CancellationToken cancellationToken = default);
    Task<bool> IsInWishlistAsync(string userId, string productId, CancellationToken cancellationToken = default);
}
