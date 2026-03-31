using Domain.Data.Models;

namespace API.Public.DTOs.Shipping.Admin;

public class AddToCartRequestDTO
{
    /// <summary>
    /// SuperFrete service ID: 1=PAC, 2=SEDEX, 17=Mini Envios, 3=Jadlog, 31=Loggi
    /// </summary>
    public int ServiceId { get; set; }

    public bool Receipt { get; set; }
    public bool OwnHand { get; set; }

    /// <summary>
    /// true = content declaration (non-commercial); false/omit = invoice
    /// </summary>
    public bool NonCommercial { get; set; }

    /// <summary>
    /// 44-digit invoice key. Required when NonCommercial is false.
    /// </summary>
    public string? InvoiceNumber { get; set; }
    public string? InvoiceKey { get; set; }

    /// <summary>
    /// Overrides the insurance value. Defaults to order total.
    /// </summary>
    public decimal? InsuranceValue { get; set; }

    public ShipmentCartOptions ToOptions() => new()
    {
        Receipt        = Receipt,
        OwnHand        = OwnHand,
        NonCommercial  = NonCommercial,
        InvoiceNumber  = InvoiceNumber,
        InvoiceKey     = InvoiceKey,
        InsuranceValue = InsuranceValue
    };
}
