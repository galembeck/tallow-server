using Domain.Data.Entities;
using Domain.Repository;
using Domain.SearchParameters;
using Domain.Services._Base;
using Microsoft.AspNetCore.Http;

namespace Domain.Services;


public abstract class IProductService : IService<Product, IProductRepository, ProductSearchParameter>
{
    public IProductService(IProductRepository repository) : base(repository) { }

    public abstract Task<Product> CreateAsync(Product product, string? actorId = null);
    public abstract Task<Product> CreateWithImageAsync(Product product, IFormFile image, List<IFormFile> additonalImages, string? actorId = null);
    public abstract Task<Product> UpdateWithImageAsync(Product product, IFormFile? image, List<IFormFile>? additionalImages, string? actorId = null);
    public abstract Task<List<Product>> GetProductsByCategoryAsync(ProductSearchParameter searchParameter, CancellationToken cancellationToken = default);
}
