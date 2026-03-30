using Domain.Data.Entities;
using Domain.Data.Models;

namespace Domain.Services;

public interface IShippingService
{
    Task<List<ShippingQuoteResponse>> CalculateShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default);
    Task<ShippingQuoteResponse> CalculateFastestShippingAsync(ShippingQuoteRequest request, CancellationToken cancellationToken = default);
    Task<List<ShippingQuoteResponse>> CalculateCartShippingAsync(CartShippingQuoteRequest request, CancellationToken cancellationToken = default);
    Task<ShippingQuoteResponse> CalculateFastestCartShippingAsync(CartShippingQuoteRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a shipping label in SuperFrete (cart + payment) and returns the label URL, tracking code and SuperFrete order ID.
    /// </summary>
    Task<ShipmentResult> CreateShipmentAsync(Order order, int serviceCode, float packageWeight, float packageHeight, float packageWidth, float packageLength, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns live tracking events for a given SuperFrete order ID.
    /// </summary>
    Task<SuperFreteTrackingResponse> GetTrackingAsync(string superFreteOrderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Streams the printable shipping label PDF from SuperFrete for the given order ID.
    /// </summary>
    Task<(Stream Content, string ContentType)> GetLabelAsync(string superFreteOrderId, CancellationToken cancellationToken = default);
}
