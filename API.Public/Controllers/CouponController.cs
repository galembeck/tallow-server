using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class CouponController : _BaseController
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        _couponService = couponService ?? throw new ArgumentNullException(nameof(couponService));
    }

    [HttpGet("admin/all")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(List<CouponResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllCoupons(CancellationToken cancellationToken = default)
    {
        try
        {
            var coupons = await _couponService.GetAllAsync(cancellationToken);
            return Ok(coupons.Select(CouponResponseDTO.ToDTO));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpPost("admin")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(CouponResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var actorId = Authenticated?.User?.Id;
            var coupon = await _couponService.CreateAsync(dto.Code, dto.DiscountPercentage, actorId, cancellationToken);
            return CreatedAtAction(nameof(GetAllCoupons), CouponResponseDTO.ToDTO(coupon));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpPut("admin/{id}")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(CouponResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCoupon(string id, [FromBody] UpdateCouponDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var actorId = Authenticated?.User?.Id;
            var coupon = await _couponService.UpdateAsync(id, dto.Code, dto.DiscountPercentage, actorId, cancellationToken);
            return Ok(CouponResponseDTO.ToDTO(coupon));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpPatch("admin/{id}/toggle")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(CouponResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ToggleCoupon(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var actorId = Authenticated?.User?.Id;
            var coupon = await _couponService.ToggleActiveAsync(id, actorId, cancellationToken);
            return Ok(CouponResponseDTO.ToDTO(coupon));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpGet("validate/{code}")]
    [AuthAttribute]
    [ProducesResponseType(typeof(CouponValidateResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidateCoupon(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var coupon = await _couponService.ValidateAsync(code, cancellationToken);

            if (coupon == null)
                return Ok(new CouponValidateResponseDTO
                {
                    IsValid = false,
                    Message = "Cupom inválido ou inativo."
                });

            return Ok(new CouponValidateResponseDTO
            {
                IsValid = true,
                Code = coupon.Code,
                DiscountPercentage = coupon.DiscountPercentage
            });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }
}
