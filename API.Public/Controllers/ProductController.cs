using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using API.Public.Validators;
using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.SearchParameters;
using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(IProductService productService) : _BaseController
{
    private readonly IProductService _productService = productService ?? throw new ArgumentNullException(nameof(productService));

    [HttpPost]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductDTO body)
    {
        try
        {
            var actorId = Authenticated?.User?.Id;

            var product = CreateProductDTO.DTOToModel(body);
            var productSaved = await _productService.CreateWithImageAsync(
                    product,
                    body.Image,
                    body.AdditionalImages,
                    actorId
                );

            var response = ProductResponseDTO.ModelToDTO(productSaved);

            response.CreatedBy = Authenticated?.User?.Name;

            return Ok(response);
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);

            throw;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var response = await _productService.GetAllAsync();

        return Ok(ProductResponseDTO.ModelToDTO(response));
    }

    [HttpGet("filter")]
    public async Task<IActionResult> GetProductsByFilter(
        [FromQuery] ProductCategory? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? searchTerm,
        [FromQuery] bool? inStock,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var searchParameter = new ProductSearchParameter
            {
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SearchTerm = searchTerm,
                InStock = inStock
            };

            var products = await _productService.GetProductsByCategoryAsync(searchParameter, cancellationToken);

            return Ok(ProductResponseDTO.ModelToDTO(products));
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);

            throw;
        }
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(string productId, CancellationToken cancellationToken = default)
    {
        var response = await _productService.GetByIdAsync(productId);

        return Ok(ProductResponseDTO.ModelToDTO(response));
    }

    [HttpPut("{productId}")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(ProductResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(string productId, [FromForm] UpdateProductDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var actorId = Authenticated?.User?.Id;

            var validator = new ProductUpdateValidator();
            await validator.ValidateAndThrowAsync(body);

            var existing = await _productService.GetByIdAsync(productId, cancellationToken);
            if (existing == null)
                return NotFound();

            var updated = UpdateProductDTO.ApplyToModel(body, existing);

            var saved = await _productService.UpdateWithImageAsync(updated, body.Image, body.AdditionalImages, actorId);

            return Ok(ProductResponseDTO.ModelToDTO(saved));
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{productId}")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProductById(string productId, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var product = await _productService.GetByIdAsync(productId);
            if (product == null)
                throw new BusinessException(BusinessErrorMessage.PRODUCT_NOT_FOUND);

            await _productService.DeleteAsync(product, userId);

            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
