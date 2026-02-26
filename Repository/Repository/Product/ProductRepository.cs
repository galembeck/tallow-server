using Domain.Data.Entities;
using Domain.Repository;
using Repository.Repository._Base;

namespace Repository.Repository;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context, context.Products) { }
}
