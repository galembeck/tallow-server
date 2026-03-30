using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
{
    public PaymentRepository(AppDbContext context) : base(context, context.Payments) { }

    public async Task<Payment?> GetByIdWithRelationsAsync(string paymentId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Payment>()
            .Include(p => p.User)
            .Include(p => p.Order)
                .ThenInclude(o => o.Items)
            .FirstOrDefaultAsync(p => p.Id == paymentId && p.DeletedAt == null, cancellationToken);
    }

    public async Task<Payment?> GetByMercadoPagoIdAsync(long mercadoPagoPaymentId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Payment>()
            .Include(p => p.User)
            .Include(p => p.Order)
            .FirstOrDefaultAsync(p => p.MercadoPagoPaymentId == mercadoPagoPaymentId, cancellationToken);
    }

    public async Task<List<Payment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Payment>()
            .Include(p => p.User)
            .Include(p => p.Order)
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Payment>()
            .Include(p => p.User)
            .Include(p => p.Order)
            .Where(p => p.Status == status)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Payment>> GetAllWithRelationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Payment>()
            .Include(p => p.User)
            .Include(p => p.Order)
                .ThenInclude(o => o.Items)
            .Where(p => p.DeletedAt == null)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
