using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using API.Public.Validators;
using Domain.Enumerators;
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

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetProductById(string productId, CancellationToken cancellationToken = default)
    {
        var response = await _productService.GetByIdAsync(productId);

        return Ok(ProductResponseDTO.ModelToDTO(response));
    }
}
