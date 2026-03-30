using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByIdWithRelationsAsync(string paymentId, CancellationToken cancellationToken = default);
    Task<Payment?> GetByMercadoPagoIdAsync(long mercadoPagoPaymentId, CancellationToken cancellationToken = default);
    Task<List<Payment>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<Payment>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default);
    Task<List<Payment>> GetAllWithRelationsAsync(CancellationToken cancellationToken = default);
}
