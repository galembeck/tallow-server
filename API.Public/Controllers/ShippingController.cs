using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.DTOs.Shipping;
using API.Public.DTOs.Shipping.Admin;
using API.Public.Filters;
using Domain.Constants;
using Domain.Data.Models;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class ShippingController(
    IShippingService shippingService,
    IProductService productService,
    IOrderService orderService) : _BaseController
{
    private readonly IShippingService _shippingService = shippingService
        ?? throw new ArgumentNullException(nameof(shippingService));
    private readonly IProductService _productService = productService
        ?? throw new ArgumentNullException(nameof(productService));
    private readonly IOrderService _orderService = orderService
        ?? throw new ArgumentNullException(nameof(orderService));

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateShipping([FromBody] ShippingQuoteRequestDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _productService.GetByIdAsync(body.ProductId, cancellationToken);

            var shippingRequest = ShippingQuoteRequestDTO.DTOToModel(
                body,
                product.Weight,
                product.Height,
                product.Width,
                product.Length,
                product.Price,
                Constant.Settings.ShippingServiceSettings.ShippingPostalCode
            );

            var quotes = await _shippingService.CalculateShippingAsync(shippingRequest, cancellationToken);

            return Ok(ShippingQuoteResponseDTO.ModelToDTO(quotes));
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    [HttpPost("calculate/fastest")]
    public async Task<IActionResult> CalculateFastestShipping([FromBody] ShippingQuoteRequestDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _productService.GetByIdAsync(body.ProductId, cancellationToken);

            var shippingRequest = ShippingQuoteRequestDTO.DTOToModel(
                body,
                product.Weight,
                product.Height,
                product.Width,
                product.Length,
                product.Price,
                Constant.Settings.ShippingServiceSettings.ShippingPostalCode
            );

            var cheapestQuote = await _shippingService.CalculateFastestShippingAsync(shippingRequest, cancellationToken);

            return Ok(ShippingQuoteResponseDTO.ModelToDTO(cheapestQuote));
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    [HttpPost("calculate/cart")]
    public async Task<IActionResult> CalculateCartShipping([FromBody] CartShippingQuoteRequestDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var cartRequest = new CartShippingQuoteRequest
            {
                FromZipCode = Constant.Settings.ShippingServiceSettings.ShippingPostalCode,
                ToZipCode = body.ToZipCode.Replace("-", "").Replace(".", "").Trim(),
                Products = new List<CartProductItem>()
            };

            foreach (var item in body.Items)
            {
                var product = await _productService.GetByIdAsync(item.ProductId, cancellationToken);

                cartRequest.Products.Add(new CartProductItem
                {
                    Quantity = item.Quantity > 0 ? item.Quantity : 1,
                    Weight = product.Weight,
                    Height = product.Height,
                    Width = product.Width,
                    Length = product.Length,
                    DeclaredValue = product.Price
                });
            }

            var quotes = await _shippingService.CalculateCartShippingAsync(cartRequest, cancellationToken);

            return Ok(ShippingQuoteResponseDTO.ModelToDTO(quotes));
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    [HttpPost("calculate/cart/fastest")]
    public async Task<IActionResult> CalculateFastestCartShipping([FromBody] CartShippingQuoteRequestDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var cartRequest = new CartShippingQuoteRequest
            {
                FromZipCode = Constant.Settings.ShippingServiceSettings.ShippingPostalCode,
                ToZipCode = body.ToZipCode.Replace("-", "").Replace(".", "").Trim(),
                Products = new List<CartProductItem>()
            };

            foreach (var item in body.Items)
            {
                var product = await _productService.GetByIdAsync(item.ProductId, cancellationToken);

                cartRequest.Products.Add(new CartProductItem
                {
                    Quantity = item.Quantity > 0 ? item.Quantity : 1,
                    Weight = product.Weight,
                    Height = product.Height,
                    Width = product.Width,
                    Length = product.Length,
                    DeclaredValue = product.Price
                });
            }

            var cheapestQuote = await _shippingService.CalculateFastestCartShippingAsync(cartRequest, cancellationToken);

            return Ok(ShippingQuoteResponseDTO.ModelToDTO(cheapestQuote));
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    /// <summary>
    /// Step 1 — Add the order to the SuperFrete cart (POST /api/v0/cart).
    /// Returns the SuperFrete order ID and price. Call /checkout next to generate the label.
    /// </summary>
    [HttpPost("admin/order/{orderId}/cart")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    public async Task<IActionResult> AddToCart(string orderId, [FromBody] AddToCartRequestDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);
            if (order is null)
                return NotFound(new { message = "Order not found." });

            var cartResponse = await _shippingService.AddOrderToCartAsync(order, body.ServiceId, body.ToOptions(), cancellationToken);

            await _orderService.UpdateOrderSuperFreteDataAsync(
                orderId,
                superFreteOrderId: cartResponse.Id,
                cancellationToken: cancellationToken);

            return Ok(new
            {
                superFreteOrderId = cartResponse.Id,
                price             = cartResponse.Price,
                status            = cartResponse.Status
            });
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    /// <summary>
    /// Step 2 — Checkout/generate the shipping label (POST /api/v0/checkout).
    /// Deducts the cost from your SuperFrete balance and returns the tracking code + print URL.
    /// </summary>
    [HttpPost("admin/order/{orderId}/checkout")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    public async Task<IActionResult> Checkout(string orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);
            if (order is null)
                return NotFound(new { message = "Order not found." });

            if (string.IsNullOrWhiteSpace(order.SuperFreteOrderId))
                return BadRequest(new { message = "Order has not been added to the SuperFrete cart yet. Call /cart first." });

            var checkoutResponse = await _shippingService.CheckoutOrderAsync(order.SuperFreteOrderId, cancellationToken);

            var purchaseOrder = checkoutResponse.Purchase?.Orders?.FirstOrDefault();

            await _orderService.UpdateOrderSuperFreteDataAsync(
                orderId,
                trackingCode: purchaseOrder?.Tracking,
                labelUrl: purchaseOrder?.Print?.Url,
                cancellationToken: cancellationToken);

            if (checkoutResponse.Success)
                await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.PREPARING, cancellationToken);

            return Ok(new
            {
                success      = checkoutResponse.Success,
                status       = checkoutResponse.Purchase?.Status,
                trackingCode = purchaseOrder?.Tracking,
                labelUrl     = purchaseOrder?.Print?.Url,
                price        = purchaseOrder?.Price,
                serviceId    = purchaseOrder?.ServiceId
            });
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    /// <summary>
    /// Step 3 — Get the PDF print URL for the shipping label (POST /api/v0/tag/print).
    /// </summary>
    [HttpPost("admin/order/{orderId}/print")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    public async Task<IActionResult> PrintLabel(string orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);
            if (order is null)
                return NotFound(new { message = "Order not found." });

            if (string.IsNullOrWhiteSpace(order.SuperFreteOrderId))
                return BadRequest(new { message = "No SuperFrete order associated with this order. Call /cart and /checkout first." });

            var printResponse = await _shippingService.PrintLabelAsync(order.SuperFreteOrderId, cancellationToken);

            // Persist the label URL if not already stored
            if (string.IsNullOrWhiteSpace(order.SuperFreteLabelUrl))
                await _orderService.UpdateOrderSuperFreteDataAsync(orderId, labelUrl: printResponse.Url, cancellationToken: cancellationToken);

            return Ok(new { labelUrl = printResponse.Url });
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    /// <summary>
    /// Track the shipment status and get full label info (GET /api/v0/order/info/{id}).
    /// </summary>
    [HttpGet("admin/order/{orderId}/track")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    public async Task<IActionResult> TrackOrder(string orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);
            if (order is null)
                return NotFound(new { message = "Order not found." });

            if (string.IsNullOrWhiteSpace(order.SuperFreteOrderId))
                return BadRequest(new { message = "No SuperFrete order associated with this order." });

            var info = await _shippingService.GetOrderInfoAsync(order.SuperFreteOrderId, cancellationToken);

            // Keep tracking code in sync
            if (!string.IsNullOrWhiteSpace(info.Tracking) && order.TrackingCode != info.Tracking)
                await _orderService.UpdateOrderSuperFreteDataAsync(orderId, trackingCode: info.Tracking, cancellationToken: cancellationToken);

            // Sync order status based on SuperFrete status
            var newStatus = info.Status switch
            {
                "posted"    => (OrderStatus?)OrderStatus.SHIPPED,
                "delivered" => OrderStatus.DELIVERED,
                _           => null
            };
            if (newStatus.HasValue && order.Status != newStatus.Value)
                await _orderService.UpdateOrderStatusAsync(orderId, newStatus.Value, cancellationToken);

            return Ok(new
            {
                superFreteOrderId = info.Id,
                status            = info.Status,
                trackingCode      = info.Tracking,
                carrier           = info.ServiceId,
                deliveryDays      = info.Delivery,
                deliveryMin       = info.DeliveryMin,
                deliveryMax       = info.DeliveryMax,
                price             = info.Price,
                weight            = info.Weight,
                dimensions        = new { height = info.Height, width = info.Width, length = info.Length },
                generatedAt       = info.GeneratedAt,
                postedAt          = info.PostedAt,
                createdAt         = info.CreatedAt,
                updatedAt         = info.UpdatedAt
            });
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }

    /// <summary>
    /// Cancel the shipment (POST /api/v0/order/cancel).
    /// Only works for labels that have not yet been posted.
    /// </summary>
    [HttpPost("admin/order/{orderId}/cancel")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    public async Task<IActionResult> CancelShipment(string orderId, [FromBody] CancelShipmentRequestDTO body, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);
            if (order is null)
                return NotFound(new { message = "Order not found." });

            if (string.IsNullOrWhiteSpace(order.SuperFreteOrderId))
                return BadRequest(new { message = "No SuperFrete order associated with this order." });

            var cancelled = await _shippingService.CancelOrderAsync(order.SuperFreteOrderId, body.Description, cancellationToken);

            if (cancelled)
            {
                await _orderService.ClearOrderShippingDataAsync(orderId, cancellationToken);
                await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.PROCESSING, cancellationToken);
            }

            return Ok(new { cancelled });
        }
        catch (Exception e)
        {
            StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            throw;
        }
    }
}
