using Domain.Data.Models;

namespace Domain.Services;

public interface IShippingService
{
    Task<List<ShippingQuoteResponse>> CalculateShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default);
    Task<ShippingQuoteResponse> CalculateFastestShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default);
    Task<List<ShippingQuoteResponse>> CalculateCartShippingAsync(CartShippingQuoteRequest request, CancellationToken cancellationToken = default);
    Task<ShippingQuoteResponse> CalculateFastestCartShippingAsync(CartShippingQuoteRequest request, CancellationToken cancellationToken = default);
}
