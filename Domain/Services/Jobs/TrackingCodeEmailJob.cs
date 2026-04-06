using Domain.Data.Entities;
using Domain.Repository;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace Domain.Services;

public class TrackingCodeEmailJob(
    IOrderRepository orderRepository,
    IShippingService shippingService,
    IEmailService emailService,
    IBackgroundJobClient backgroundJobClient,
    ILogger<TrackingCodeEmailJob> logger) : ITrackingCodeEmailJob
{
    private readonly IOrderRepository _orderRepository = orderRepository;
    private readonly IShippingService _shippingService = shippingService;
    private readonly IEmailService _emailService = emailService;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
    private readonly ILogger<TrackingCodeEmailJob> _logger = logger;

    /// <summary>
    /// Maximum number of polling attempts before giving up (24 × 5 min = 2 hours).
    /// </summary>
    private const int MaxAttempts = 24;

    public async Task SendWhenTrackingAvailableAsync(string orderId, int attempt)
    {
        var order = await _orderRepository.GetAsync(orderId);
        if (order is null)
        {
            _logger.LogWarning("[TrackingJob] Pedido {OrderId} não encontrado. Encerrando.", orderId);
            return;
        }

        // 1 — Tracking code already stored (e.g., set by the polling endpoint)
        if (!string.IsNullOrWhiteSpace(order.TrackingCode))
        {
            _logger.LogInformation("[TrackingJob] Código de rastreio encontrado no BD para o pedido {OrderId}: {Code}",
                orderId, order.TrackingCode);

            await SendEmailAsync(order, order.TrackingCode);
            return;
        }

        // 2 — Try to fetch live data directly from SuperFrete
        if (!string.IsNullOrWhiteSpace(order.SuperFreteOrderId))
        {
            try
            {
                var info = await _shippingService.GetOrderInfoAsync(order.SuperFreteOrderId);

                if (!string.IsNullOrWhiteSpace(info?.Tracking))
                {
                    _logger.LogInformation("[TrackingJob] Código de rastreio obtido do SuperFrete para o pedido {OrderId}: {Code}",
                        orderId, info.Tracking);

                    // Persist so the admin page also shows it
                    await _orderRepository.UpdatePartialAsync(
                        new Order { Id = orderId },
                        o => o.TrackingCode = info.Tracking);

                    await SendEmailAsync(order, info.Tracking);
                    return;
                }

                _logger.LogInformation("[TrackingJob] SuperFrete ainda sem código para o pedido {OrderId} (tentativa {Attempt}/{Max}).",
                    orderId, attempt + 1, MaxAttempts);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[TrackingJob] Erro ao consultar SuperFrete para o pedido {OrderId}. Tentativa {Attempt}/{Max}.",
                    orderId, attempt + 1, MaxAttempts);
            }
        }

        // 3 — Code not yet available — reschedule if within the attempt limit
        if (attempt < MaxAttempts)
        {
            _backgroundJobClient.Schedule<ITrackingCodeEmailJob>(
                j => j.SendWhenTrackingAvailableAsync(orderId, attempt + 1),
                TimeSpan.FromMinutes(5));
        }
        else
        {
            _logger.LogError("[TrackingJob] Número máximo de tentativas atingido para o pedido {OrderId}. E-mail de envio não foi disparado.", orderId);
        }
    }

    private async Task SendEmailAsync(Order order, string trackingCode)
    {
        await _emailService.SendOrderShippedEmailAsync(
            order.BuyerName,
            order.BuyerEmail,
            order.Id,
            trackingCode,
            order.ShippingService ?? "Correios");

        _logger.LogInformation("[TrackingJob] E-mail de envio disparado para o pedido {OrderId}.", order.Id);
    }
}
