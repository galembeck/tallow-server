using Domain.Data.Models;

namespace Domain.Services;

public interface IShippingService
{
    Task<List<ShippingQuoteResponse>> CalculateShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default);
    Task<ShippingQuoteResponse> CalculateCheapestShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default);
}
