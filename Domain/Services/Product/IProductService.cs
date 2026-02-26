using Domain.Data.Entities;
using Domain.Repository;
using Domain.SearchParameters;
using Domain.Services._Base;

namespace Domain.Services;


public abstract class IProductService : IService<Product, IProductRepository, ProductSearchParameter>
{
    public IProductService(IProductRepository repository) : base(repository) { }

    public abstract Task<Product> CreateAsync(Product product, string? actorId = null);
}
