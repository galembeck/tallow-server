using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : _BaseController
{
    private readonly IOrderService _orderService;
    private readonly IShippingService _shippingService;
    private readonly IAdminNotificationService _notificationService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        IOrderService orderService,
        IShippingService shippingService,
        IAdminNotificationService notificationService,
        ILogger<OrderController> logger)
    {
        _orderService        = orderService        ?? throw new ArgumentNullException(nameof(orderService));
        _shippingService     = shippingService     ?? throw new ArgumentNullException(nameof(shippingService));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _logger              = logger              ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet("admin/all")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(List<OrderAdminSummaryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken = default)
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
            return Ok(OrderAdminSummaryDTO.ToDTO(orders));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpGet("admin/{id}")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(OrderResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrderAdmin(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);

            if (order == null)
                return NotFound();

            return Ok(OrderResponseDTO.ToDTO(order));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    /// <summary>
    /// Marks the order as being prepared (PROCESSING).
    /// Called when the admin clicks "Preparar Pedido".
    /// </summary>
    [HttpPatch("admin/{id}/prepare")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PrepareOrder(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
            if (order is null)
                return NotFound();

            await _orderService.UpdateOrderStatusAsync(id, OrderStatus.PROCESSING, cancellationToken);

            return Ok(new { message = "Pedido marcado como em preparação." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    /// <summary>
    /// Runs the SuperFrete shipping flow:
    ///   1. Adds the order to the SuperFrete cart
    ///   2. Checks out (deducts balance, assigns tracking code on most carriers)
    ///   3. Prints the label (generates the PDF)
    ///   4. Persists superFreteOrderId, tracking code and label URL
    ///   5. Sets order status to SHIPPED
    ///   6. Broadcasts real-time notification to admin hub
    ///
    /// Tracking code resolution is intentionally deferred to the polling
    /// endpoint (GET /order/admin/{id}/shipping) so this endpoint responds
    /// as fast as possible. The frontend polls every 6 s until the code arrives.
    /// </summary>
    [HttpPatch("admin/{id}/ship")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ShipOrder(string id, [FromBody] ShipOrderRequestDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
            if (order is null)
                return NotFound();

            // Step 1 – add to SuperFrete cart
            var cartResponse = await _shippingService.AddOrderToCartAsync(order, dto.ServiceId, null, cancellationToken);
            _logger.LogInformation("[Ship] Cart criado: superFreteOrderId={Id}", cartResponse.Id);

            await _orderService.UpdateOrderSuperFreteDataAsync(
                id,
                superFreteOrderId: cartResponse.Id,
                cancellationToken: cancellationToken);

            // Step 2 – checkout (deducts balance; tracking code is present here for most carriers)
            var checkoutResponse = await _shippingService.CheckoutOrderAsync(cartResponse.Id, cancellationToken);
            var purchaseOrder    = checkoutResponse.Purchase?.Orders?.FirstOrDefault();
            _logger.LogInformation("[Ship] Checkout: success={Ok} tracking={T} labelUrl={U}",
                checkoutResponse.Success, purchaseOrder?.Tracking, purchaseOrder?.Print?.Url);

            // Step 3 – print/generate the label PDF
            var printResponse = await _shippingService.PrintLabelAsync(cartResponse.Id, cancellationToken);
            _logger.LogInformation("[Ship] Print: labelUrl={U}", printResponse.Url);

            // Persist whatever we have now. If tracking is still null after checkout + print,
            // GET /order/admin/{id}/shipping will pick it up via its live SuperFrete refresh.
            var trackingCode = !string.IsNullOrWhiteSpace(purchaseOrder?.Tracking) ? purchaseOrder!.Tracking : null;
            var labelUrl     = !string.IsNullOrWhiteSpace(printResponse.Url)       ? printResponse.Url
                             : !string.IsNullOrWhiteSpace(purchaseOrder?.Print?.Url) ? purchaseOrder!.Print!.Url
                             : null;

            await _orderService.UpdateOrderSuperFreteDataAsync(
                id,
                trackingCode: trackingCode,
                labelUrl:     labelUrl,
                cancellationToken: cancellationToken);

            // Step 4 – set status to SHIPPED
            await _orderService.UpdateOrderStatusAsync(id, OrderStatus.SHIPPED, cancellationToken);

            // Step 5 – notify admin hub
            await _notificationService.NotifyOrderShippedAsync(id, trackingCode ?? string.Empty, cancellationToken);

            return Ok(new
            {
                superFreteOrderId = cartResponse.Id,
                trackingCode,
                labelUrl
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[Ship] Erro ao processar envio do pedido {OrderId}", id);
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    /// <summary>
    /// Returns the shipping details stored on the order (tracking code, label URL,
    /// SuperFrete IDs) plus a live status refresh from SuperFrete when available.
    /// Called by the admin to get full shipping info after the order has been shipped.
    /// </summary>
    [HttpGet("admin/{id}/shipping")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetShippingDetails(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
            if (order is null)
                return NotFound();

            // If we have a SuperFrete order ID, fetch live data and keep local info in sync
            if (!string.IsNullOrWhiteSpace(order.SuperFreteOrderId))
            {
                try
                {
                    var info = await _shippingService.GetOrderInfoAsync(order.SuperFreteOrderId, cancellationToken);

                    // Sync tracking if SuperFrete now has it and we don't
                    if (!string.IsNullOrWhiteSpace(info.Tracking) && string.IsNullOrWhiteSpace(order.TrackingCode))
                    {
                        await _orderService.UpdateOrderSuperFreteDataAsync(id, trackingCode: info.Tracking, cancellationToken: cancellationToken);
                        order.TrackingCode = info.Tracking;
                    }

                    // Sync status: posted → SHIPPED, delivered → DELIVERED
                    var liveStatus = info.Status switch
                    {
                        "posted"    => (OrderStatus?)OrderStatus.SHIPPED,
                        "delivered" => OrderStatus.DELIVERED,
                        _           => null
                    };
                    if (liveStatus.HasValue && order.Status != liveStatus.Value)
                    {
                        await _orderService.UpdateOrderStatusAsync(id, liveStatus.Value, cancellationToken);
                        order.Status = liveStatus.Value;
                    }

                    return Ok(new
                    {
                        orderId           = order.Id,
                        status            = order.Status,
                        superFreteOrderId = order.SuperFreteOrderId,
                        trackingCode      = order.TrackingCode,
                        labelUrl          = order.SuperFreteLabelUrl,
                        shippedAt         = order.ShippedAt,
                        deliveredAt       = order.DeliveredAt,
                        live = new
                        {
                            superFreteStatus = info.Status,
                            trackingCode     = info.Tracking,
                            carrier          = info.ServiceId,
                            deliveryDays     = info.Delivery,
                            deliveryMin      = info.DeliveryMin,
                            deliveryMax      = info.DeliveryMax,
                            postedAt         = info.PostedAt,
                            generatedAt      = info.GeneratedAt
                        }
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[Shipping] Não foi possível buscar info ao vivo do SuperFrete para o pedido {OrderId}", id);
                }
            }

            // Fallback — return only what's stored locally
            return Ok(new
            {
                orderId           = order.Id,
                status            = order.Status,
                superFreteOrderId = order.SuperFreteOrderId,
                trackingCode      = order.TrackingCode,
                labelUrl          = order.SuperFreteLabelUrl,
                shippedAt         = order.ShippedAt,
                deliveredAt       = order.DeliveredAt,
                live              = (object?)null
            });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    /// <summary>
    /// Proxies the SuperFrete shipping label PDF through the server, adding the
    /// required Bearer token so the browser can open it directly without CORS/auth issues.
    /// Returns the PDF as an inline attachment named "etiqueta-{orderId}.pdf".
    /// </summary>
    [HttpGet("admin/{id}/label")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DownloadLabel(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);
            if (order is null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(order.SuperFreteOrderId))
                return BadRequest(new { message = "Pedido ainda não foi enviado pelo SuperFrete." });

            // Always request a fresh print URL — the stored label URL is short-lived
            // and will return 500 if accessed after expiry.
            var printResponse = await _shippingService.PrintLabelAsync(order.SuperFreteOrderId, cancellationToken);

            if (string.IsNullOrWhiteSpace(printResponse.Url))
                return BadRequest(new { message = "Etiqueta ainda não disponível no SuperFrete." });

            var (bytes, contentType) = await _shippingService.DownloadLabelAsync(printResponse.Url, cancellationToken);

            // Return inline so the browser renders the PDF in the new tab
            Response.Headers["Content-Disposition"] = "inline";
            return File(bytes, contentType);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpPost]
    [AuthAttribute]
    [ProducesResponseType(typeof(OrderResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var order = await _orderService.CreateOrderFromCartAsync(userId, dto.CartId, dto.BuyerInfo, dto.ShippingInfo, cancellationToken);

            await _notificationService.NotifyOrderCreatedAsync(order.Id, order.BuyerName, order.TotalAmount, cancellationToken);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, OrderResponseDTO.ToDTO(order));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            throw;
        }
    }

    [HttpGet("{id}")]
    [AuthAttribute]
    [ProducesResponseType(typeof(OrderResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrder(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);

            if (order == null)
                return NotFound();

            return Ok(OrderResponseDTO.ToDTO(order));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            throw;
        }
    }

    [HttpGet("user/me")]
    [AuthAttribute]
    [ProducesResponseType(typeof(List<OrderResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var orders = await _orderService.GetUserOrdersAsync(userId, cancellationToken);

            return Ok(orders.Select(o => OrderResponseDTO.ToDTO(o)));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            throw;
        }
    }
}
