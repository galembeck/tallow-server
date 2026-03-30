using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class WishlistController : _BaseController
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService ?? throw new ArgumentNullException(nameof(wishlistService));
    }

    [HttpGet]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.CLIENT, ProfileType.ADMIN)]
    [ProducesResponseType(typeof(List<WishlistItemDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMyWishlist(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;
            var items = await _wishlistService.GetUserWishlistAsync(userId, cancellationToken);
            return Ok(WishlistItemDTO.ToDTO(items));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpPost("{productId}")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.CLIENT, ProfileType.ADMIN)]
    [ProducesResponseType(typeof(WishlistItemDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddToWishlist(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;
            var item = await _wishlistService.AddToWishlistAsync(userId, productId, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, WishlistItemDTO.ToDTO(item));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpDelete("{productId}")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.CLIENT, ProfileType.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveFromWishlist(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;
            await _wishlistService.RemoveFromWishlistAsync(userId, productId, cancellationToken);
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpGet("{productId}/check")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.CLIENT, ProfileType.ADMIN)]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CheckWishlist(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;
            var isInWishlist = await _wishlistService.IsInWishlistAsync(userId, productId, cancellationToken);
            return Ok(isInWishlist);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }
}
