using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : _BaseController
{
    private readonly IOrderService _orderService;
    private readonly IShippingService _shippingService;
    private readonly IAdminNotificationService _notificationService;

    public OrderController(
        IOrderService orderService,
        IShippingService shippingService,
        IAdminNotificationService notificationService)
    {
        _orderService          = orderService          ?? throw new ArgumentNullException(nameof(orderService));
        _shippingService       = shippingService       ?? throw new ArgumentNullException(nameof(shippingService));
        _notificationService   = notificationService   ?? throw new ArgumentNullException(nameof(notificationService));
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
    /// Runs the full SuperFrete shipping flow in one step:
    ///   1. Adds the order to the SuperFrete cart
    ///   2. Checks out (deducts balance, assigns tracking code on most carriers)
    ///   3. Prints the label (generates the PDF — required to confirm the tracking code)
    ///   4. Fetches definitive order info (guarantees tracking code is populated)
    ///   5. Persists superFreteOrderId, tracking code and label URL
    ///   6. Sets order status to SHIPPED
    ///   7. Broadcasts real-time notification to admin hub
    ///
    /// Called when the admin clicks "Marcar como Enviado".
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

            await _orderService.UpdateOrderSuperFreteDataAsync(
                id,
                superFreteOrderId: cartResponse.Id,
                cancellationToken: cancellationToken);

            // Step 2 – checkout (deducts balance; tracking may already be present here)
            var checkoutResponse = await _shippingService.CheckoutOrderAsync(cartResponse.Id, cancellationToken);
            var purchaseOrder    = checkoutResponse.Purchase?.Orders?.FirstOrDefault();

            // Step 3 – print label (confirms the shipment and ensures tracking is generated)
            var printResponse = await _shippingService.PrintLabelAsync(cartResponse.Id, cancellationToken);

            // Step 4 – fetch definitive order info (tracking code is always present here)
            var orderInfo    = await _shippingService.GetOrderInfoAsync(cartResponse.Id, cancellationToken);
            var trackingCode = orderInfo.Tracking
                               ?? purchaseOrder?.Tracking
                               ?? string.Empty;
            var labelUrl     = printResponse.Url
                               ?? purchaseOrder?.Print?.Url;

            await _orderService.UpdateOrderSuperFreteDataAsync(
                id,
                trackingCode: trackingCode,
                labelUrl:     labelUrl,
                cancellationToken: cancellationToken);

            // Step 5 – set status to SHIPPED
            await _orderService.UpdateOrderStatusAsync(id, OrderStatus.SHIPPED, cancellationToken);

            // Step 6 – notify admin hub
            await _notificationService.NotifyOrderShippedAsync(id, trackingCode, cancellationToken);

            return Ok(new
            {
                superFreteOrderId = cartResponse.Id,
                trackingCode,
                labelUrl
            });
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
