using Domain.Enumerators;

namespace API.Public.DTOs;

public class PayGiftCardDTO
{
    public PaymentMethod PaymentType { get; set; }

    // Credit card only
    public string? Token { get; set; }
    public int? Installments { get; set; }
    public string? IssuerId { get; set; }
    public string? PaymentMethodId { get; set; }

    public PayerDTO Payer { get; set; } = new();
    public DateTime? DateOfExpiration { get; set; }
}
