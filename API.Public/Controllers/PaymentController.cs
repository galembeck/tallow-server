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

            var payerFirstName = dto.Payer.FirstName ?? order.BuyerName?.Split(' ').FirstOrDefault() ?? order.BuyerName;
            var payerLastName = dto.Payer.LastName ?? string.Join(" ", order.BuyerName?.Split(' ').Skip(1) ?? Array.Empty<string>()) ?? "";
            var payerIdentification = dto.Payer.Identification != null
                ? new IdentificationRequest { Type = dto.Payer.Identification.Type, Number = dto.Payer.Identification.Number }
                : new IdentificationRequest { Type = "CPF", Number = order.BuyerDocument };

            var phoneDigits = order.BuyerCellphone?.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
            var additionalInfo = new AdditionalInfoRequest
            {
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Items = order.Items?.Select(item => new AdditionalItemRequest
                {
                    Id = item.Id,
                    Title = item.ProductName,
                    PictureUrl = item.ProductImageUrl,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                }).ToList(),
                Payer = new AdditionalPayerRequest
                {
                    FirstName = payerFirstName,
                    LastName = payerLastName,
                    Phone = !string.IsNullOrEmpty(phoneDigits) ? new PhoneRequest
                    {
                        AreaCode = phoneDigits.Length >= 2 ? phoneDigits.Substring(0, 2) : null,
                        Number = phoneDigits.Length > 2 ? phoneDigits.Substring(2) : phoneDigits
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
            };

            var sharedMetadata = new Dictionary<string, string>
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
            };

            var description = dto.Description ?? $"Pedido #{order.Id.Substring(0, 8)} - {order.BuyerName}";

            MercadoPagoPaymentRequest mpRequest;

            switch (dto.PaymentType)
            {
                case Domain.Enumerators.PaymentMethod.PIX:
                    mpRequest = new MercadoPagoPaymentRequest
                    {
                        TransactionAmount = dto.TransactionAmount,
                        PaymentMethodId = "pix",
                        Installments = 1,
                        StatementDescriptor = "TERRAETALLOW",
                        Description = description,
                        ExternalReference = dto.OrderId,
                        DateOfExpiration = dto.DateOfExpiration ?? DateTime.UtcNow.AddMinutes(30),
                        Capture = true,
                        BinaryMode = false,
                        Payer = new PayerRequest
                        {
                            Email = dto.Payer.Email,
                            FirstName = payerFirstName,
                            LastName = payerLastName,
                            Identification = payerIdentification
                        },
                        AdditionalInfo = additionalInfo,
                        Metadata = sharedMetadata
                    };
                    break;

                case Domain.Enumerators.PaymentMethod.BOLETO:
                    mpRequest = new MercadoPagoPaymentRequest
                    {
                        TransactionAmount = dto.TransactionAmount,
                        PaymentMethodId = "boleto",
                        Installments = 1,
                        StatementDescriptor = "TERRAETALLOW",
                        Description = description,
                        ExternalReference = dto.OrderId,
                        DateOfExpiration = dto.DateOfExpiration ?? DateTime.UtcNow.AddDays(3),
                        Capture = true,
                        BinaryMode = false,
                        Payer = new PayerRequest
                        {
                            Email = dto.Payer.Email,
                            FirstName = payerFirstName,
                            LastName = payerLastName,
                            Identification = payerIdentification
                        },
                        AdditionalInfo = additionalInfo,
                        Metadata = sharedMetadata
                    };
                    break;

                default: // CREDIT_CARD
                    mpRequest = new MercadoPagoPaymentRequest
                    {
                        Token = dto.Token,
                        TransactionAmount = dto.TransactionAmount,
                        StatementDescriptor = "TERRAETALLOW",
                        PaymentMethodId = dto.PaymentMethodId,
                        Installments = dto.Installments,
                        IssuerId = dto.IssuerId,
                        Description = description,
                        ExternalReference = dto.OrderId,
                        DateOfExpiration = dto.DateOfExpiration,
                        Capture = true,
                        BinaryMode = true,
                        Payer = new PayerRequest
                        {
                            Email = dto.Payer.Email,
                            FirstName = payerFirstName,
                            LastName = payerLastName,
                            Identification = payerIdentification
                        },
                        AdditionalInfo = additionalInfo,
                        Metadata = sharedMetadata
                    };
                    break;
            }

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

    [HttpGet("admin/all")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    [ProducesResponseType(typeof(List<PaymentAdminDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllPayments(CancellationToken cancellationToken = default)
    {
        try
        {
            var payments = await _paymentService.GetAllPaymentsAsync(cancellationToken);
            return Ok(PaymentAdminDTO.ToDTO(payments));
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
