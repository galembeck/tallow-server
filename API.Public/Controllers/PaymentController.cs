using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.Filters;
using Domain.Data.Models;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentController : _BaseController
{
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;

    public PaymentController(
        IPaymentService paymentService,
        IOrderService orderService)
    {
        _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        _orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
    }

    [HttpPost("process")]
    [AuthAttribute]
    [ProducesResponseType(typeof(PaymentResponseDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePayment(
        [FromBody] CreatePaymentDTO dto,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var order = await _orderService.GetOrderByIdAsync(dto.OrderId, cancellationToken);

            if (order == null)
                return BadRequest(BusinessErrorMessage.ORDER_NOT_FOUND.ToString());

            var mpRequest = new MercadoPagoPaymentRequest
            {
                Token = dto.Token,
                TransactionAmount = dto.TransactionAmount,
                StatementDescriptor = "TERRAETALLOW",
                PaymentMethodId = dto.PaymentMethodId,

                Payer = new PayerRequest
                {
                    Email = dto.Payer.Email,
                    FirstName = dto.Payer.FirstName ?? order.BuyerName?.Split(' ').FirstOrDefault() ?? order.BuyerName,
                    LastName = dto.Payer.LastName ?? string.Join(" ", order.BuyerName?.Split(' ').Skip(1) ?? Array.Empty<string>()) ?? "",
                    Identification = dto.Payer.Identification != null ? new IdentificationRequest
                    {
                        Type = dto.Payer.Identification.Type,
                        Number = dto.Payer.Identification.Number
                    } : new IdentificationRequest
                    {
                        Type = "CPF",
                        Number = order.BuyerDocument
                    }
                },

                AdditionalInfo = new AdditionalInfoRequest
                {
                    IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Items = order.Items?.Select(item => new AdditionalItemRequest
                    {
                        Id = item.Id,
                        Title = item.ProductName,
                        //Description = item.ProductDescription,
                        PictureUrl = item.ProductImageUrl,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        //Type = item.ProductCategory,
                    }).ToList(),
                    Payer = new AdditionalPayerRequest
                    {
                        FirstName = dto.Payer.FirstName ?? order.BuyerName?.Split(' ').FirstOrDefault() ?? order.BuyerName,
                        LastName = dto.Payer.LastName ?? string.Join(" ", order.BuyerName?.Split(' ').Skip(1) ?? Array.Empty<string>()) ?? "",
                        Phone = !string.IsNullOrEmpty(order.BuyerCellphone) ? new PhoneRequest
                        {
                            AreaCode = order.BuyerCellphone.Length >= 2
                            ? order.BuyerCellphone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Substring(0, 2)
                            : null,
                            Number = order.BuyerCellphone.Length > 2
                            ? order.BuyerCellphone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Substring(2)
                            : order.BuyerCellphone
                        } : null,
                        Address = new AddressRequest
                        {
                            ZipCode = order.ShippingZipcode?.Replace("-", ""),
                            StreetName = order.ShippingAddress,
                            StreetNumber = order.ShippingNumber,
                        },
                    },
                    Shipments = new AdditionalShipmentRequest
                    {
                        ReceiverAddress = new ReceiverAddressRequest
                        {
                            ZipCode = order.ShippingZipcode?.Replace("-", ""),
                            StateName = order.ShippingState,
                            CityName = order.ShippingCity,
                            StreetName = order.ShippingAddress,
                            StreetNumber = order.ShippingNumber,
                        },
                    }
                },

                Installments = dto.Installments,
                IssuerId = dto.IssuerId,
                Description = dto.Description ?? $"Pedido #{order.Id.Substring(0, 8)} - {order.BuyerName}",
                ExternalReference = dto.OrderId,

                DateOfExpiration = dto.DateOfExpiration,

                Capture = true,
                BinaryMode = true,

                //NotificationUrl = $"{Constant.Settings.BaseUrl}/webhook/mercadopago",

                Metadata = new Dictionary<string, string>
                {
                    { "order_id", order.Id },
                    { "user_id", userId },
                    { "platform", "Terra & Tallow" },
                    { "shipping_service", order.ShippingService ?? "N/A" },
                    { "buyer_name", order.BuyerName },
                    { "buyer_document", order.BuyerDocument },
                    { "buyer_email", order.BuyerEmail },
                    { "buyer_phone", order.BuyerCellphone ?? "N/A" },
                    { "shipping_city", order.ShippingCity },
                    { "shipping_state", order.ShippingState },
                    { "items_count", order.Items?.Count.ToString() ?? "0" },
                    { "subtotal_amount", order.SubTotalAmount.ToString("F2") },
                    { "shipping_amount", order.ShippingAmount.ToString("F2") },
                    { "total_amount", order.TotalAmount.ToString("F2") },
                    { "order_created_at", order.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") },
                    { "client_ip", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown" },
                    { "user_agent", HttpContext.Request.Headers["User-Agent"].ToString() }
                }
            };

            var payment = await _paymentService.CreatePaymentAsync(userId, dto.OrderId, mpRequest, cancellationToken);

            if (payment.Status == PaymentStatus.APPROVED)
            {
                await _orderService.UpdateOrderStatusAsync(dto.OrderId, OrderStatus.PAYMENT_APPROVED, cancellationToken);
            }
            else if (payment.Status == PaymentStatus.PENDING || payment.Status == PaymentStatus.IN_PROCESS)
            {
                await _orderService.UpdateOrderStatusAsync(dto.OrderId, OrderStatus.PAYMENT_PENDING, cancellationToken);
            }
            else if (payment.Status == PaymentStatus.REJECTED || payment.Status == PaymentStatus.CANCELLED)
            {
                await _orderService.UpdateOrderStatusAsync(dto.OrderId, OrderStatus.CANCELLED, cancellationToken);
            }

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, PaymentResponseDTO.ToDTO(payment));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
        }
    }

    [HttpGet("{id}")]
    [AuthAttribute]
    [ProducesResponseType(typeof(PaymentResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPayment(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id, cancellationToken);

            if (payment == null)
                return NotFound();

            return Ok(PaymentResponseDTO.ToDTO(payment));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            throw;
        }
    }

    [HttpGet("user/me")]
    [AuthAttribute]
    [ProducesResponseType(typeof(List<PaymentResponseDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMyPayments(CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;

            var payments = await _paymentService.GetUserPaymentsAsync(userId, cancellationToken);

            return Ok(payments.Select(PaymentResponseDTO.ToDTO));
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status400BadRequest, e.Message);
            throw;
        }
    }
}
