using Domain.Enumerators;
using Domain.Repository._Base;
using Domain.Data.Entities;

namespace Domain.Repository;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByIdWithItemsAsync(string orderId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<Order?> GetByPaymentIdAsync(string paymentId, CancellationToken cancellationToken = default);
    Task<List<Order>> GetAllWithRelationsAsync(CancellationToken cancellationToken = default);
}
