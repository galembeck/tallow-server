using Domain.Data.Entities;

namespace API.Public.DTOs;

public class PaymentAdminDTO
{
    // ---- Identity ----
    public string Id { get; set; }
    public long? MercadoPagoPaymentId { get; set; }
    public string? OrderId { get; set; }

    // ---- Status ----
    public string Status { get; set; }

    /// <summary>
    /// Raw MP status_detail — human-readable rejection/approval reason.
    /// E.g.: "accredited", "cc_rejected_insufficient_amount",
    /// "cc_rejected_bad_filled_security_code", "pending_waiting_payment", etc.
    /// </summary>
    public string? StatusDetail { get; set; }

    // ---- Payment method ----
    public string PaymentMethod { get; set; }    // CREDIT_CARD | PIX | BOLETO
    public string PaymentMethodId { get; set; }  // visa, mastercard, pix, boleto…
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
    public string? PayerDocument { get; set; }   // CPF
    public string? PayerPhone { get; set; }

    // ---- Order summary ----
    public int OrderItemsCount { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingState { get; set; }

    // ---- MP technical fields ----
    public string? AuthorizationCode { get; set; }
    public string? ExternalReference { get; set; }
    public bool? LiveMode { get; set; }

    // ---- Dates ----
    public DateTimeOffset DateCreated { get; set; }
    public DateTime? DateApproved { get; set; }
    public DateTime? DateLastUpdated { get; set; }
    public DateTime? DateOfExpiration { get; set; }



    public static PaymentAdminDTO ToDTO(Payment payment)
    {
        return new PaymentAdminDTO
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
            ShippingAmount = payment.ShippingAmount,
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

            AuthorizationCode = payment.AuthorizationCode,
            ExternalReference = payment.ExternalReference,
            LiveMode = payment.LiveMode,

            DateCreated = payment.CreatedAt,
            DateApproved = payment.DateApproved,
            DateLastUpdated = payment.DateLastUpdated,
            DateOfExpiration = payment.DateOfExpiration,
        };
    }

    public static List<PaymentAdminDTO> ToDTO(IEnumerable<Payment> payments)
        => payments.Select(ToDTO).ToList();
}
