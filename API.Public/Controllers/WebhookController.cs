using API.Public.Controllers._Base;
using Domain.Data.Models;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class WebhookController : _BaseController
{
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly IAdminNotificationService _notificationService;
    private readonly ILogger<WebhookController> _logger;

    public WebhookController(
        IPaymentService paymentService,
        IOrderService orderService,
        IAdminNotificationService notificationService,
        ILogger<WebhookController> logger)
    {
        _paymentService      = paymentService      ?? throw new ArgumentNullException(nameof(paymentService));
        _orderService        = orderService        ?? throw new ArgumentNullException(nameof(orderService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger              = logger              ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("mercadopago")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MercadoPagoWebhook(
        [FromBody] MercadoPagoWebhookNotification notification,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Webhook recebido: {Notification}", JsonSerializer.Serialize(notification));

            if (notification.Type != "payment")
            {
                _logger.LogWarning("Tipo de notificação não suportado: {Type}", notification.Type);
                return Ok();
            }

            var paymentId = long.Parse(notification.Data.Id);

            var payment = await _paymentService.UpdatePaymentStatusAsync(paymentId, cancellationToken);

            if (payment.OrderId != null)
            {
                if (payment.Status == PaymentStatus.APPROVED)
                {
                    await _orderService.UpdateOrderStatusAsync(payment.OrderId, OrderStatus.PAYMENT_APPROVED, cancellationToken);

                    await _notificationService.NotifyPaymentApprovedAsync(
                        payment.OrderId,
                        paymentId.ToString(),
                        payment.TransactionAmount,
                        cancellationToken);
                }
                else if (payment.Status == PaymentStatus.REJECTED || payment.Status == PaymentStatus.CANCELLED)
                {
                    await _orderService.UpdateOrderStatusAsync(payment.OrderId, OrderStatus.CANCELLED, cancellationToken);

                    await _notificationService.NotifyPaymentDeclinedAsync(
                        payment.OrderId,
                        paymentId.ToString(),
                        cancellationToken);
                }
            }

            _logger.LogInformation("Pagamento {PaymentId} atualizado com sucesso", paymentId);

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erro ao processar webhook do Mercado Pago");
            return BadRequest(e.Message);
        }
    }
}
