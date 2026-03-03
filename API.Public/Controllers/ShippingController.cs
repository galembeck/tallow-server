using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.DTOs.Shipping;
using Domain.Constants;
using Domain.Data.Models;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class ShippingController(
    IShippingService shippingService,
    IProductService productService) : _BaseController
{
    private readonly IShippingService _shippingService = shippingService 
        ?? throw new ArgumentNullException(nameof(shippingService));
    private readonly IProductService _productService = productService 
        ?? throw new ArgumentNullException(nameof(productService));

    [HttpPost("calculate")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateShipping([FromBody] ShippingQuoteRequestDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _productService.GetByIdAsync(body.ProductId, cancellationToken);

            var shippingRequest = ShippingQuoteRequestDTO.DTOToModel(
                body,
                product.Weight,
                product.Height,
                product.Width,
                product.Length,
                product.Price,
                Constant.Settings.ShippingServiceSettings.ShippingPostalCode
            );

            var quotes = await _shippingService.CalculateShippingAsync(shippingRequest, cancellationToken);

            return Ok(ShippingQuoteResponseDTO.ModelToDTO(quotes));
        } catch (Exception e) 
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    [HttpPost("calculate/fastest")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateFastestShipping([FromBody] ShippingQuoteRequestDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _productService.GetByIdAsync(body.ProductId, cancellationToken);

            var shippingRequest = ShippingQuoteRequestDTO.DTOToModel(
                body,
                product.Weight,
                product.Height,
                product.Width,
                product.Length,
                product.Price,
                Constant.Settings.ShippingServiceSettings.ShippingPostalCode
            );

            var cheapestQuote = await _shippingService.CalculateFastestShippingAsync(shippingRequest, cancellationToken);

            return Ok(ShippingQuoteResponseDTO.ModelToDTO(cheapestQuote));
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    [HttpPost("calculate/cart")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateCartShipping([FromBody] CartShippingQuoteRequestDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var cartRequest = new CartShippingQuoteRequest
            {
                FromZipCode = Constant.Settings.ShippingServiceSettings.ShippingPostalCode,
                ToZipCode = body.ToZipCode.Replace("-", "").Replace(".", "").Trim(),
                Products = new List<CartProductItem>()
            };

            foreach (var item in body.Items)
            {
                var product = await _productService.GetByIdAsync(item.ProductId, cancellationToken);

                cartRequest.Products.Add(new CartProductItem
                {
                    Quantity = item.Quantity > 0 ? item.Quantity : 1,
                    Weight = product.Weight,
                    Height = product.Height,
                    Width = product.Width,
                    Length = product.Length,
                    DeclaredValue = product.Price
                });
            }

            var quotes = await _shippingService.CalculateCartShippingAsync(cartRequest, cancellationToken);

            return Ok(ShippingQuoteResponseDTO.ModelToDTO(quotes));
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    [HttpPost("calculate/cart/fastest")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateFastestCartShipping([FromBody] CartShippingQuoteRequestDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var cartRequest = new CartShippingQuoteRequest
            {
                FromZipCode = Constant.Settings.ShippingServiceSettings.ShippingPostalCode,
                ToZipCode = body.ToZipCode.Replace("-", "").Replace(".", "").Trim(),
                Products = new List<CartProductItem>()
            };

            foreach (var item in body.Items)
            {
                var product = await _productService.GetByIdAsync(item.ProductId, cancellationToken);

                cartRequest.Products.Add(new CartProductItem
                {
                    Quantity = item.Quantity > 0 ? item.Quantity : 1,
                    Weight = product.Weight,
                    Height = product.Height,
                    Width = product.Width,
                    Length = product.Length,
                    DeclaredValue = product.Price
                });
            }

            var cheapestQuote = await _shippingService.CalculateFastestCartShippingAsync(cartRequest, cancellationToken);

            return Ok(ShippingQuoteResponseDTO.ModelToDTO(cheapestQuote));
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }
}
