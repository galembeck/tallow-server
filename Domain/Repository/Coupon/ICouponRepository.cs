using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface ICouponRepository : IRepository<Coupon>
{
    Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}
