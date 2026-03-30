using Domain.Data.Entities;

namespace API.Public.DTOs;

public class WishlistItemDTO
{
    public string Id { get; set; }
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductImageUrl { get; set; }
    public decimal ProductPrice { get; set; }
    public int ProductStock { get; set; }
    public DateTimeOffset AddedAt { get; set; }

    public static WishlistItemDTO ToDTO(WishlistItem item) => new()
    {
        Id = item.Id,
        ProductId = item.ProductId,
        ProductName = item.Product?.Name,
        ProductImageUrl = item.Product?.ImageUrl,
        ProductPrice = item.Product?.Price ?? 0,
        ProductStock = item.Product?.StockAmount ?? 0,
        AddedAt = item.CreatedAt
    };

    public static List<WishlistItemDTO> ToDTO(IEnumerable<WishlistItem> items)
        => items.Select(ToDTO).ToList();
}
