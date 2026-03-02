using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;

namespace Domain.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly IProductRepository _productRepository;

    public CartService(
        ICartRepository cartRepository,
        ICartItemRepository cartItemRepository,
        IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _productRepository = productRepository;
    }

    public async Task<Cart> GetOrCreateCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);

        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            await _cartRepository.InsertAsync(cart, userId);
        }

        return cart;
    }

    public async Task<Cart> AddItemToCartAsync(string userId, string productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new BusinessException("Quantity must be greater than 0.");

        var product = await _productRepository.GetAsync(productId, cancellationToken);

        if (product == null)
            throw new BusinessException(BusinessErrorMessage.NOT_FOUND);

        if (product.StockAmount < quantity)
            throw new BusinessException("The requested amount/quantity is not available in stock.");

        var cart = await GetOrCreateCartAsync(userId, cancellationToken);

        var existingItem = await _cartItemRepository.GetItemByProductIdAsync(cart.Id, productId, cancellationToken);

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;

            if (existingItem.Quantity > product.StockAmount)
                throw new BusinessException("The total amount/quantity exceeds the availability in stock.");

            await _cartItemRepository.UpdateAsync(existingItem, userId);
        }
        else
        {
            var newItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price
            };

            await _cartItemRepository.InsertAsync(newItem.WithoutRelations(newItem), userId);
        }

        return await _cartRepository.GetCartWithItemsAsync(cart.Id, cancellationToken);
    }

    public async Task<Cart> UpdateItemQuantityAsync(string userId, string productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new BusinessException("Quantity must be greater than zero.");

        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);

        if (cart == null)
            throw new BusinessException("Cart not found.");

        var item = await _cartItemRepository.GetItemByProductIdAsync(cart.Id, productId, cancellationToken);

        if (item == null)
            throw new BusinessException("Item not found in cart.");

        var product = await _productRepository.GetAsync(productId, cancellationToken);

        if (quantity > product.StockAmount)
            throw new BusinessException("The requested amount/quantity is not available in stock.");

        item.Quantity = quantity;
        await _cartItemRepository.UpdateAsync(item, userId);

        return await _cartRepository.GetCartWithItemsAsync(cart.Id, cancellationToken);
    }

    public async Task<Cart> RemoveItemFromCartAsync(string userId, string productId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);
        if (cart == null)
            throw new BusinessException("Cart not found.");

        var item = await _cartItemRepository.GetItemByProductIdAsync(cart.Id, productId, cancellationToken);
        if (item == null)
            throw new BusinessException("Item not found in cart.");

        await _cartItemRepository.HardDeleteAsync(item);

        return await _cartRepository.GetCartWithItemsAsync(cart.Id, cancellationToken);
    }

    public async Task<Cart> ClearCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);
        if (cart == null)
            throw new BusinessException("Cart not found.");

        var items = await _cartItemRepository.GetItemsByCartIdAsync(cart.Id, cancellationToken);

        foreach (var item in items)
        {
            await _cartItemRepository.HardDeleteAsync(item);
        }

        return await _cartRepository.GetCartWithItemsAsync(cart.Id, cancellationToken);
    }

    public async Task<Cart> GetCartByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetActiveCartByUserIdAsync(userId, cancellationToken);

        if (cart == null)
            return await GetOrCreateCartAsync(userId, cancellationToken);

        return cart;
    }
}