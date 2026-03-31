namespace Domain.Data.Models;

public class ShipmentCartOptions
{
    public bool Receipt { get; set; }
    public bool OwnHand { get; set; }
    public bool NonCommercial { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceKey { get; set; }
    public decimal? InsuranceValue { get; set; }
}
