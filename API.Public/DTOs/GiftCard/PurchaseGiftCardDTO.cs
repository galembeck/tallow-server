using Domain.Enumerators;

namespace API.Public.DTOs;

public class PurchaseGiftCardDTO
{
    public decimal Amount { get; set; }
    public PaymentMethod PaymentType { get; set; }
    public PayerDTO Payer { get; set; }

    // Credit card fields (optional)
    public string? Token { get; set; }
    public string? PaymentMethodId { get; set; }
    public int? Installments { get; set; }
    public string? IssuerId { get; set; }
    public DateTime? DateOfExpiration { get; set; }
    public string? Description { get; set; }
}
