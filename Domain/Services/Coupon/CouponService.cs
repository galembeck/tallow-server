using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;

namespace Domain.Services;

public class CouponService(ICouponRepository couponRepository) : ICouponService
{
    private readonly ICouponRepository _couponRepository = couponRepository;

    public async Task<List<Coupon>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var all = await _couponRepository.GetAllAsync(cancellationToken);
        return all.OrderByDescending(c => c.CreatedAt).ToList();
    }

    public async Task<Coupon> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _couponRepository.GetAsync(id, cancellationToken);
    }

    public async Task<Coupon?> ValidateAsync(string code, CancellationToken cancellationToken = default)
    {
        var coupon = await _couponRepository.GetByCodeAsync(code, cancellationToken);

        if (coupon == null || !coupon.IsActive)
            return null;

        if (coupon.ExpiresAt.HasValue && coupon.ExpiresAt.Value <= DateTime.UtcNow)
            return null;

        return coupon;
    }

    public async Task<Coupon> CreateAsync(string code, decimal discountPercentage, DateTime? expiresAt = null, string? actorId = null, CancellationToken cancellationToken = default)
    {
        var existing = await _couponRepository.GetByCodeAsync(code, cancellationToken);
        if (existing != null)
            throw new BusinessException("Já existe um cupom com esse código.");

        var coupon = new Coupon
        {
            Code = code.ToUpper().Trim(),
            DiscountPercentage = discountPercentage,
            IsActive = true,
            UsageCount = 0,
            ExpiresAt = expiresAt?.ToUniversalTime()
        };

        return await _couponRepository.InsertAsync(coupon, actorId);
    }

    public async Task<Coupon> UpdateAsync(string id, string code, decimal discountPercentage, DateTime? expiresAt, string? actorId = null, CancellationToken cancellationToken = default)
    {
        var existing = await _couponRepository.GetByCodeAsync(code, cancellationToken);
        if (existing != null && existing.Id != id)
            throw new BusinessException("Já existe um cupom com esse código.");

        return await _couponRepository.UpdatePartialAsync(
            new Coupon { Id = id },
            c =>
            {
                c.Code = code.ToUpper().Trim();
                c.DiscountPercentage = discountPercentage;
                c.ExpiresAt = expiresAt?.ToUniversalTime();
            },
            actorId);
    }

    public async Task<Coupon> ToggleActiveAsync(string id, string? actorId = null, CancellationToken cancellationToken = default)
    {
        return await _couponRepository.UpdatePartialAsync(
            new Coupon { Id = id },
            c => c.IsActive = !c.IsActive,
            actorId);
    }

    public async Task IncrementUsageAsync(string id, CancellationToken cancellationToken = default)
    {
        await _couponRepository.UpdatePartialAsync(
            new Coupon { Id = id },
            c => c.UsageCount += 1);
    }
}
