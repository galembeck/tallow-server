using Domain.Data.Models;
using Domain.Data.Entities;

namespace Domain.Services;

public interface IPaymentService
{
    Task<Payment> CreatePaymentAsync(
        string userId,
        string? orderId,
        MercadoPagoPaymentRequest request,
        CancellationToken cancellationToken = default);

    Task<Payment?> GetPaymentByIdAsync(string paymentId, CancellationToken cancellationToken = default);
    Task<Payment?> GetPaymentDetailAsync(string paymentId, CancellationToken cancellationToken = default);
    Task<List<Payment>> GetUserPaymentsAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<Payment>> GetAllPaymentsAsync(CancellationToken cancellationToken = default);

    Task<Payment> UpdatePaymentStatusAsync(long mercadoPagoPaymentId, CancellationToken cancellationToken = default);
}
