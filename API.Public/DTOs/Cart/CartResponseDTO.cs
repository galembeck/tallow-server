using API.Public.DTOs._Base;
using Domain.Data.Entities;

namespace API.Public.DTOs;

public class CartResponseDTO : PublicBaseDTO<Cart>
{
    public string UserId { get; set; }

    public List<CartItemResponseDTO> Items { get; set; }

    public decimal TotalAmount { get; set; }
    public int TotalItems { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }



    public CartResponseDTO() { }

    public CartResponseDTO(Cart cart)
    {
        if (cart == null) return;

        Id = cart.Id;
        UserId = cart.UserId;
        Items = cart.Items?.Select(i => new CartItemResponseDTO(i)).ToList() ?? new List<CartItemResponseDTO>();
        TotalAmount = cart.TotalAmount;
        TotalItems = cart.TotalItems;
        UpdatedAt = cart.UpdatedAt;
    }

    public static CartResponseDTO ModelToDTO(Cart cart) => cart == null ? null : new CartResponseDTO(cart);
}
