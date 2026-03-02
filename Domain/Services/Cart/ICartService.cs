using Domain.Data.Entities;

namespace Domain.Services;

public interface ICartService
{
    Task<Cart> GetOrCreateCartAsync(string userId, CancellationToken cancellationToken = default);
    Task<Cart> AddItemToCartAsync(string userId, string productId, int quantity, CancellationToken cancellationToken = default);
    Task<Cart> UpdateItemQuantityAsync(string userId, string productId, int quantity, CancellationToken cancellationToken = default);
    Task<Cart> RemoveItemFromCartAsync(string userId, string productId, CancellationToken cancellationToken = default);
    Task<Cart> ClearCartAsync(string userId, CancellationToken cancellationToken = default);
    Task<Cart> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}