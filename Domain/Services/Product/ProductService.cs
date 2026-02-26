using Domain.Data.Entities;
using Domain.Repository;

namespace Domain.Services;

public class ProductService(
    IProductRepository repository,
    IProductRepository productRepository) : IProductService(repository)
{
    public override async Task<Product> CreateAsync(Product product, string? actorId = null)
    {
        var productSaved = await _Repository.InsertAsync(product.WithoutRelations(product), actorId);

        return productSaved;
    }
}
