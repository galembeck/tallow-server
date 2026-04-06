using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context) : base(context, context.Orders) { }

    public async Task<Order?> GetByIdWithItemsAsync(string orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Order>()
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Payments)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);
    }

    public async Task<List<Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Order>()
            .Include(o => o.User)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Payments)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Order>()
            .Include(o => o.User)
            .Include(o => o.Items)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByPaymentIdAsync(string paymentId, CancellationToken cancellationToken = default)
    {
        var payment = await _context.Set<Payment>()
            .Include(p => p.Order)
                .ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(p => p.Id == paymentId, cancellationToken);

        return payment?.Order;
    }

    public async Task<List<Order>> GetAllWithRelationsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<Order>()
            .Include(o => o.User)
            .Include(o => o.Items)
            .Include(o => o.Payments)
            .Where(o => o.DeletedAt == null)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}