using Domain.Data.Entities;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class CouponRepository : BaseRepository<Coupon>, ICouponRepository
{
    public CouponRepository(AppDbContext context) : base(context, context.Coupons) { }

    public async Task<Coupon?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _entity
            .Where(c => c.Code.ToUpper() == code.ToUpper() && c.DeletedAt == null)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);
    }
}
