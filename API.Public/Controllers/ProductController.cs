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
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDTO body)
    {
        try
        {
            var actorId = Authenticated?.User?.Id;

            await new ProductCreationValidator().ValidateAndThrowAsync(body);

            var product = CreateProductDTO.DTOToModel(body);
            var productSaved = await _productService.CreateAsync(product, actorId);

            var response = ProductResponseDTO.ModelToDTO(productSaved);

            response.CreatedBy = Authenticated?.User?.Name;

            return Ok(response);
        } catch (Exception e)
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
}
