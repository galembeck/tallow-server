namespace API.Public.DTOs;

public class CreatePaymentDTO
{
    public string OrderId { get; set; }
    public string? Token { get; set; }
    public decimal TransactionAmount { get; set; }
    public int Installments { get; set; }
    public string PaymentMethodId { get; set; }
    public string? IssuerId { get; set; }
    public PayerDTO Payer { get; set; }
    public string? Description { get; set; }
    public DateTime? DateOfExpiration { get; set; }
}