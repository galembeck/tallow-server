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
public class GiftCardController : _BaseController
{
    private readonly IGiftCardService _giftCardService;
    private readonly IPaymentService _paymentService;

    public GiftCardController(
        IGiftCardService giftCardService,
        IPaymentService paymentService)
    {
        _giftCardService = giftCardService;
        _paymentService  = paymentService;
    }

    // GET /gift-card/admin/all
    [HttpGet("admin/all")]
    [AuthAttribute]
    [AuthorizeAttribute(ProfileType.ADMIN)]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        try
        {
            var cards = await _giftCardService.GetAllAsync(ct);
            return Ok(cards.Select(g => GiftCardResponseDTO.ToDTO(g, includeUserInfo: true)));
        }
        catch (Exception e) { return StatusCode(400, e.Message); }
    }

    // GET /gift-card/user/me
    [HttpGet("user/me")]
    [AuthAttribute]
    public async Task<IActionResult> GetMine(CancellationToken ct = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;
            var cards = await _giftCardService.GetByUserIdAsync(userId!, ct);
            return Ok(cards.Select(g => GiftCardResponseDTO.ToDTO(g)));
        }
        catch (Exception e) { return StatusCode(400, e.Message); }
    }

    // GET /gift-card/{id}
    [HttpGet("{id}")]
    [AuthAttribute]
    public async Task<IActionResult> GetById(string id, CancellationToken ct = default)
    {
        try
        {
            var gc = await _giftCardService.GetByIdAsync(id, ct);
            if (gc == null) return NotFound();
            return Ok(GiftCardResponseDTO.ToDTO(gc));
        }
        catch (Exception e) { return StatusCode(400, e.Message); }
    }

    // GET /gift-card/{id}/refresh — checks payment status and activates if approved
    [HttpGet("{id}/refresh")]
    [AuthAttribute]
    public async Task<IActionResult> RefreshStatus(string id, CancellationToken ct = default)
    {
        try
        {
            var gc = await _giftCardService.GetByIdAsync(id, ct);
            if (gc == null) return NotFound();

            if (gc.Status == GiftCardStatus.PENDING && gc.Payment?.MercadoPagoPaymentId.HasValue == true)
            {
                var updatedPayment = await _paymentService.UpdatePaymentStatusAsync(gc.Payment.MercadoPagoPaymentId!.Value, ct);
                if (updatedPayment.Status == PaymentStatus.APPROVED)
                {
                    await _giftCardService.ActivateAsync(id, ct);
                    gc = await _giftCardService.GetByIdAsync(id, ct);
                }
            }

            return Ok(GiftCardResponseDTO.ToDTO(gc!));
        }
        catch (Exception e) { return StatusCode(400, e.Message); }
    }

    // POST /gift-card/purchase
    [HttpPost("purchase")]
    [AuthAttribute]
    public async Task<IActionResult> Purchase([FromBody] PurchaseGiftCardDTO dto, CancellationToken ct = default)
    {
        try
        {
            var userId = Authenticated?.User?.Id;
            var user   = Authenticated?.User;

            // 1. Create pending gift card
            var gc = await _giftCardService.CreateAsync(userId!, dto.Amount, userId, ct);

            // 2. Build payer info
            var payerFirstName = dto.Payer.FirstName ?? user?.Name?.Split(' ').FirstOrDefault() ?? "";
            var payerLastName  = dto.Payer.LastName  ?? string.Join(" ", user?.Name?.Split(' ').Skip(1) ?? Array.Empty<string>());
            var payerIdentification = dto.Payer.Identification != null
                ? new IdentificationRequest { Type = dto.Payer.Identification.Type, Number = dto.Payer.Identification.Number }
                : new IdentificationRequest { Type = "CPF", Number = user?.Document ?? "" };

            var additionalInfo = new AdditionalInfoRequest
            {
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Items =
                [
                    new AdditionalItemRequest
                    {
                        Id        = gc.Id,
                        Title     = $"Vale Presente Terra & Tallow — R$ {dto.Amount:F2}",
                        Quantity  = 1,
                        UnitPrice = dto.Amount,
                    }
                ],
                Payer = new AdditionalPayerRequest
                {
                    FirstName = payerFirstName,
                    LastName  = payerLastName
                }
            };

            var description = dto.Description ?? $"Vale Presente — R$ {dto.Amount:F2} — Terra & Tallow";

            MercadoPagoPaymentRequest mpRequest = dto.PaymentType switch
            {
                PaymentMethod.PIX => new MercadoPagoPaymentRequest
                {
                    TransactionAmount   = dto.Amount,
                    PaymentMethodId     = "pix",
                    Installments        = 1,
                    StatementDescriptor = "TERRAETALLOW",
                    Description         = description,
                    ExternalReference   = gc.Id,
                    DateOfExpiration    = dto.DateOfExpiration ?? DateTime.UtcNow.AddMinutes(30),
                    Capture             = true,
                    BinaryMode          = false,
                    Payer = new PayerRequest
                    {
                        Email          = dto.Payer.Email,
                        FirstName      = payerFirstName,
                        LastName       = payerLastName,
                        Identification = payerIdentification
                    },
                    AdditionalInfo = additionalInfo
                },
                PaymentMethod.BOLETO => new MercadoPagoPaymentRequest
                {
                    TransactionAmount   = dto.Amount,
                    PaymentMethodId     = "boleto",
                    Installments        = 1,
                    StatementDescriptor = "TERRAETALLOW",
                    Description         = description,
                    ExternalReference   = gc.Id,
                    DateOfExpiration    = dto.DateOfExpiration ?? DateTime.UtcNow.AddDays(3),
                    Capture             = true,
                    BinaryMode          = false,
                    Payer = new PayerRequest
                    {
                        Email          = dto.Payer.Email,
                        FirstName      = payerFirstName,
                        LastName       = payerLastName,
                        Identification = payerIdentification
                    },
                    AdditionalInfo = additionalInfo
                },
                _ => new MercadoPagoPaymentRequest // CREDIT_CARD
                {
                    Token               = dto.Token,
                    TransactionAmount   = dto.Amount,
                    StatementDescriptor = "TERRAETALLOW",
                    PaymentMethodId     = dto.PaymentMethodId!,
                    Installments        = dto.Installments ?? 1,
                    IssuerId            = dto.IssuerId,
                    Description         = description,
                    ExternalReference   = gc.Id,
                    Capture             = true,
                    BinaryMode          = true,
                    Payer = new PayerRequest
                    {
                        Email          = dto.Payer.Email,
                        FirstName      = payerFirstName,
                        LastName       = payerLastName,
                        Identification = payerIdentification
                    },
                    AdditionalInfo = additionalInfo
                }
            };

            // 3. Create payment (orderId = null for gift cards)
            var payment = await _paymentService.CreatePaymentAsync(userId!, null, mpRequest, ct);

            // 4. Link payment to gift card
            await _giftCardService.LinkPaymentAsync(gc.Id, payment.Id, ct);

            // 5. Activate only if payment was immediately approved (e.g. credit card)
            if (payment.Status == PaymentStatus.APPROVED)
                await _giftCardService.ActivateAsync(gc.Id, ct);

            // 6. Re-fetch so DTO includes payment data (PixQrCode etc.)
            gc = await _giftCardService.GetByIdAsync(gc.Id, ct);

            return CreatedAtAction(nameof(GetById), new { id = gc!.Id }, GiftCardResponseDTO.ToDTO(gc));
        }
        catch (Exception e) { return StatusCode(400, e.Message); }
    }
}
