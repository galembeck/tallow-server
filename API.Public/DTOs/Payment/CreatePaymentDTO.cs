using Domain.Enumerators;

namespace API.Public.DTOs;

public class CreatePaymentDTO
{
    public string OrderId { get; set; }

    /// <summary>
    /// Payment method type: CREDIT_CARD, PIX or BOLETO
    /// </summary>
    public PaymentMethod PaymentType { get; set; }

    // ---- Credit card only ----
    public string? Token { get; set; }
    public int Installments { get; set; } = 1;
    public string? IssuerId { get; set; }

    // ---- Credit card: card brand (e.g. "visa"). PIX/Boleto: leave empty, filled automatically ----
    public string? PaymentMethodId { get; set; }

    public decimal TransactionAmount { get; set; }
    public PayerDTO Payer { get; set; }
    public string? Description { get; set; }
    public DateTime? DateOfExpiration { get; set; }
}