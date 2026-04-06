using Domain.Data.Entities;

namespace Domain.Services;

public interface ICouponService
{
    Task<List<Coupon>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Coupon> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Coupon?> ValidateAsync(string code, CancellationToken cancellationToken = default);
    Task<Coupon> CreateAsync(string code, decimal discountPercentage, DateTime? expiresAt = null, string? actorId = null, CancellationToken cancellationToken = default);
    Task<Coupon> UpdateAsync(string id, string code, decimal discountPercentage, DateTime? expiresAt, string? actorId = null, CancellationToken cancellationToken = default);
    Task IncrementUsageAsync(string id, CancellationToken cancellationToken = default);
    Task<Coupon> ToggleActiveAsync(string id, string? actorId = null, CancellationToken cancellationToken = default);
}
