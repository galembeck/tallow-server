using Domain.Data.Entities;

namespace API.Public.DTOs;

/// <summary>
/// Lightweight DTO for the admin payments list/table.
/// Navigate to the detail screen for full payer and order information.
/// </summary>
public class PaymentAdminDTO
{
    public string Id { get; set; }
    public long? MercadoPagoPaymentId { get; set; }
    public string? OrderId { get; set; }

    public string Status { get; set; }
    public string? StatusDetail { get; set; }

    public string PaymentMethod { get; set; }
    public string PaymentMethodId { get; set; }
    public int? Installments { get; set; }

    public decimal TransactionAmount { get; set; }
    public decimal? ShippingAmount { get; set; }
    public string? CurrencyId { get; set; }

    public string? PayerName { get; set; }
    public string? PayerDocument { get; set; }

    public bool? LiveMode { get; set; }

    public DateTimeOffset DateCreated { get; set; }
    public DateTime? DateApproved { get; set; }
    public DateTime? DateLastUpdated { get; set; }



    public static PaymentAdminDTO ToDTO(Payment payment) => new()
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
        CurrencyId = payment.CurrencyId ?? "BRL",

        PayerName = payment.Order?.BuyerName ?? payment.User?.Name,
        PayerDocument = payment.Order?.BuyerDocument,

        LiveMode = payment.LiveMode,

        DateCreated = payment.CreatedAt,
        DateApproved = payment.DateApproved,
        DateLastUpdated = payment.DateLastUpdated,
    };

    public static List<PaymentAdminDTO> ToDTO(IEnumerable<Payment> payments)
        => payments.Select(ToDTO).ToList();
}
