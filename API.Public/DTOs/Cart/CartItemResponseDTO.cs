using API.Public.DTOs._Base;
using Domain.Data.Entities;

namespace API.Public.DTOs;

public class CartItemResponseDTO : PublicBaseDTO<CartItem>
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public string ProductImageUrl { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal SubTotal { get; set; }
    
    public int StockAvailable { get; set; }

    

    public CartItemResponseDTO() { }

    public CartItemResponseDTO(CartItem item)
    {
        if (item == null) return;

        Id = item.Id;
        ProductId = item.ProductId;
        ProductName = item.Product?.Name ?? string.Empty;
        ProductDescription = item.Product?.Description ?? string.Empty;
        ProductImageUrl = item.Product?.ImageUrl ?? string.Empty;
        Quantity = item.Quantity;
        UnitPrice = item.UnitPrice;
        SubTotal = item.SubTotal;
        StockAvailable = item.Product?.StockAmount ?? 0;
    }

    public static List<CartItemResponseDTO> ModelToDTO(IEnumerable<CartItem> items)
        => items.Select(item => new CartItemResponseDTO(item)).ToList();
}
