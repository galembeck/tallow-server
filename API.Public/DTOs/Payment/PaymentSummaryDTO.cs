using Domain.Enumerators;
using System.Text.Json.Serialization;

namespace API.Public.DTOs;

public class PaymentSummaryDTO
{
    public string Id { get; set; }
    public string Status { get; set; }
    public string PaymentMethod { get; set; }
    public decimal TransactionAmount { get; set; }
}
