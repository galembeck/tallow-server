using Domain.Data.Entities;

namespace API.Public.DTOs;

/// <summary>
/// Full payment detail DTO for the admin payment detail screen.
/// Includes payer info, order summary and all MP fields.
/// </summary>
public class PaymentDetailDTO
{
    // ---- Identity ----
    public string Id { get; set; }
    public long? MercadoPagoPaymentId { get; set; }
    public string? OrderId { get; set; }

    // ---- Status ----
    public string Status { get; set; }

    /// <summary>
    /// Raw MP status_detail — approval/rejection reason.
    /// E.g.: "accredited", "cc_rejected_insufficient_amount",
    /// "cc_rejected_bad_filled_security_code", "pending_waiting_payment".
    /// </summary>
    public string? StatusDetail { get; set; }

    // ---- Payment method ----
    public string PaymentMethod { get; set; }
    public string PaymentMethodId { get; set; }
    public int? Installments { get; set; }

    // ---- Amounts ----
    public decimal TransactionAmount { get; set; }
    public decimal? ShippingAmount { get; set; }
    public decimal OrderSubTotal { get; set; }
    public decimal OrderTotal { get; set; }
    public string? CurrencyId { get; set; }

    // ---- Payer ----
    public string? PayerName { get; set; }
    public string? PayerEmail { get; set; }
    public string? PayerDocument { get; set; }
    public string? PayerPhone { get; set; }

    // ---- Order summary ----
    public int OrderItemsCount { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingState { get; set; }

    // ---- PIX ----
    public string? PixQrCode { get; set; }
    public string? PixQrCodeBase64 { get; set; }
    public string? PixCopyPaste { get; set; }

    // ---- Boleto ----
    public string? BoletoUrl { get; set; }
    public string? BoletoBarcode { get; set; }

    // ---- MP technical ----
    public string? AuthorizationCode { get; set; }
    public string? ExternalReference { get; set; }
    public bool? LiveMode { get; set; }
    public string? StatementDescriptor { get; set; }

    // ---- Dates ----
    public DateTimeOffset DateCreated { get; set; }
    public DateTime? DateApproved { get; set; }
    public DateTime? DateLastUpdated { get; set; }
    public DateTime? DateOfExpiration { get; set; }



    public static PaymentDetailDTO ToDTO(Payment payment) => new()
    {
        Id = payment.Id,
        MercadoPagoPaymentId = payment.MercadoPagoPaymentId,
        OrderId = payment.OrderId,

        Status = payment.Status.ToString(),
        StatusDetail = payment.StatusDetail,

        PaymentMethod = payment.PaymentMethod.ToString(),
        PaymentMethodId = payment.MercadoPagoPaymentMethodId,
        Installments = payment.Installments,

        TransactionAmount = payment.TransactionAmount,
        ShippingAmount = payment.Order?.ShippingAmount ?? payment.ShippingAmount,
        OrderSubTotal = payment.Order?.SubTotalAmount ?? 0,
        OrderTotal = payment.Order?.TotalAmount ?? 0,
        CurrencyId = payment.CurrencyId ?? "BRL",

        PayerName = payment.Order?.BuyerName ?? payment.User?.Name,
        PayerEmail = payment.Order?.BuyerEmail ?? payment.User?.Email,
        PayerDocument = payment.Order?.BuyerDocument,
        PayerPhone = payment.Order?.BuyerCellphone,

        OrderItemsCount = payment.Order?.Items?.Count ?? 0,
        ShippingCity = payment.Order?.ShippingCity,
        ShippingState = payment.Order?.ShippingState,

        PixQrCode = payment.PixQrCode,
        PixQrCodeBase64 = payment.PixQrCodeBase64,
        PixCopyPaste = payment.PixCopyPaste,

        BoletoUrl = payment.BoletoUrl,
        BoletoBarcode = payment.BoletoBarcode,

        AuthorizationCode = payment.AuthorizationCode,
        ExternalReference = payment.ExternalReference,
        LiveMode = payment.LiveMode,
        StatementDescriptor = payment.StatementDescriptor,

        DateCreated = payment.CreatedAt,
        DateApproved = payment.DateApproved,
        DateLastUpdated = payment.DateLastUpdated,
        DateOfExpiration = payment.DateOfExpiration,
    };
}
