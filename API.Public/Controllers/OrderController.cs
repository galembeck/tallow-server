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

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
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

    [HttpGet("admin/status/{status}")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(List<OrderAdminSummaryDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrdersByStatus(Domain.Enumerators.OrderStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var orders = await _orderService.GetAllByStatusAsync(status, cancellationToken);
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

    [HttpPatch("admin/{id}/prepare")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(OrderAdminSummaryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkAsPreparing(string id, [FromBody] OrderAdminActionDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.MarkAsPreparingAsync(id, dto.AdminNotes, cancellationToken);
            return Ok(OrderAdminSummaryDTO.ToDTO(order));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpPost("admin/{id}/ship")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(OrderAdminSummaryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ShipOrder(string id, [FromBody] ShipOrderDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.ShipOrderAsync(
                id,
                dto.ServiceCode,
                dto.PackageWeight,
                dto.PackageHeight,
                dto.PackageWidth,
                dto.PackageLength,
                dto.AdminNotes,
                cancellationToken);

            return Ok(OrderAdminSummaryDTO.ToDTO(order));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpPatch("admin/{id}/deliver")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(OrderAdminSummaryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkAsDelivered(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.MarkAsDeliveredAsync(id, cancellationToken);
            return Ok(OrderAdminSummaryDTO.ToDTO(order));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpPatch("admin/{id}/cancel")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(OrderAdminSummaryDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelOrder(string id, [FromBody] OrderAdminActionDTO dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.CancelOrderAsync(id, dto.AdminNotes, cancellationToken);
            return Ok(OrderAdminSummaryDTO.ToDTO(order));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpGet("admin/{id}/tracking")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(OrderTrackingDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrderTracking(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var tracking = await _orderService.GetOrderTrackingAsync(id, cancellationToken);
            return Ok(OrderTrackingDTO.FromSuperFrete(tracking));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpGet("{id}/tracking")]
    [AuthAttribute]
    [ProducesResponseType(typeof(OrderTrackingDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMyOrderTracking(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var tracking = await _orderService.GetOrderTrackingAsync(id, cancellationToken);
            return Ok(OrderTrackingDTO.FromSuperFrete(tracking));
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