using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class CartController(ICartService cartService) : _BaseController
{
    private readonly ICartService _cartService = cartService ?? throw new ArgumentNullException(nameof(cartService));

    [HttpGet]
    [AuthAttribute]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var cart = await _cartService.GetCartByUserIdAsync(userId, cancellationToken);

            return Ok(CartResponseDTO.ModelToDTO(cart));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    [HttpPost("items")]
    [AuthAttribute]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> AddItemToCart([FromBody] AddToCartDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var cart = await _cartService.AddItemToCartAsync(userId, body.ProductId, body.Quantity, cancellationToken);

            return Ok(CartResponseDTO.ModelToDTO(cart));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            throw;
        }
    }

    [HttpPut("items")]
    [AuthAttribute]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> UpdateItemQuantity([FromBody] UpdateCartItemDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var cart = await _cartService.UpdateItemQuantityAsync(userId, body.ProductId, body.Quantity, cancellationToken);

            return Ok(CartResponseDTO.ModelToDTO(cart));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            throw;
        }
    }

    [HttpDelete("items/{productId}")]
    [AuthAttribute]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> RemoveItemFromCart(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var cart = await _cartService.RemoveItemFromCartAsync(userId, productId, cancellationToken);

            return Ok(CartResponseDTO.ModelToDTO(cart));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            throw;
        }
    }

    [HttpDelete]
    [AuthAttribute]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var cart = await _cartService.ClearCartAsync(userId, cancellationToken);

            return Ok(CartResponseDTO.ModelToDTO(cart));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            throw;
        }
    }
}