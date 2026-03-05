using Domain.Data.Models;

namespace Domain.Services;

public interface IMercadoPagoService
{
    Task<MercadoPagoPaymentResponse> CreatePaymentAsync(MercadoPagoPaymentRequest request, CancellationToken cancellationToken = default);
    Task<MercadoPagoPaymentResponse> GetPaymentAsync(long paymentId, CancellationToken cancellationToken = default);
}
