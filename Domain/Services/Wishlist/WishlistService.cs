using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;

namespace Domain.Services;

public class WishlistService(
    IWishlistRepository wishlistRepository,
    IProductRepository productRepository) : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository = wishlistRepository;
    private readonly IProductRepository _productRepository = productRepository;

    public async Task<List<WishlistItem>> GetUserWishlistAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _wishlistRepository.GetByUserIdAsync(userId, cancellationToken);
    }

    public async Task<WishlistItem> AddToWishlistAsync(string userId, string productId, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetAsync(productId, cancellationToken);
        if (product == null)
            throw new BusinessException(BusinessErrorMessage.PRODUCT_NOT_FOUND);

        var existing = await _wishlistRepository.GetByUserAndProductAsync(userId, productId, cancellationToken);
        if (existing != null)
            return existing;

        var item = new WishlistItem
        {
            UserId = userId,
            ProductId = productId
        };

        var saved = await _wishlistRepository.InsertAsync(item, userId);
        saved.Product = product;

        return saved;
    }

    public async Task RemoveFromWishlistAsync(string userId, string productId, CancellationToken cancellationToken = default)
    {
        var item = await _wishlistRepository.GetByUserAndProductAsync(userId, productId, cancellationToken);

        if (item == null)
            throw new BusinessException(BusinessErrorMessage.NOT_FOUND);

        await _wishlistRepository.DeleteAsync(item, userId);
    }

    public async Task<bool> IsInWishlistAsync(string userId, string productId, CancellationToken cancellationToken = default)
    {
        var item = await _wishlistRepository.GetByUserAndProductAsync(userId, productId, cancellationToken);
        return item != null;
    }
}
