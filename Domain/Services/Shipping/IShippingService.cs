using Domain.Data.Entities;
using Domain.Data.Models;

namespace Domain.Services;

public interface IShippingService
{
    Task<List<ShippingQuoteResponse>> CalculateShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default);
    Task<ShippingQuoteResponse> CalculateFastestShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default);
    Task<List<ShippingQuoteResponse>> CalculateCartShippingAsync(CartShippingQuoteRequest request, CancellationToken cancellationToken = default);
    Task<ShippingQuoteResponse> CalculateFastestCartShippingAsync(CartShippingQuoteRequest request, CancellationToken cancellationToken = default);

    // Admin: SuperFrete label lifecycle
    Task<SuperFreteCartResponse> AddOrderToCartAsync(Order order, int serviceId, ShipmentCartOptions? options = null, CancellationToken cancellationToken = default);
    Task<SuperFreteCheckoutResponse> CheckoutOrderAsync(string superFreteOrderId, CancellationToken cancellationToken = default);
    Task<SuperFretePrintResponse> PrintLabelAsync(string superFreteOrderId, CancellationToken cancellationToken = default);
    Task<SuperFreteOrderInfoResponse> GetOrderInfoAsync(string superFreteOrderId, CancellationToken cancellationToken = default);
    Task<bool> CancelOrderAsync(string superFreteOrderId, string description = "Cancelado pelo usuário", CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads the shipping label PDF from SuperFrete using the stored API token
    /// and returns the raw bytes + MIME type so the controller can proxy it to the client.
    /// </summary>
    Task<(byte[] Bytes, string ContentType)> DownloadLabelAsync(string labelUrl, CancellationToken cancellationToken = default);
}
