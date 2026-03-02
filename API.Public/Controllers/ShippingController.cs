using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.DTOs.Shipping;
using Domain.Constants;
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

    [HttpPost("calculate/cheapest")]
    [AllowAnonymous]
    public async Task<IActionResult> CalculateCheapestShipping([FromBody] ShippingQuoteRequestDTO body, CancellationToken cancellationToken = default)
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

            var cheapestQuote = await _shippingService.CalculateCheapestShippingAsync(shippingRequest, cancellationToken);

            return Ok(ShippingQuoteResponseDTO.ModelToDTO(cheapestQuote));
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }
}
