using Domain.Enumerators;
using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class MercadoPagoPaymentResponse
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("date_created")]
    public DateTime DateCreated { get; set; }

    [JsonPropertyName("date_approved")]
    public DateTime? DateApproved { get; set; }

    [JsonPropertyName("date_last_updated")]
    public DateTime? DateLastUpdated { get; set; }

    [JsonPropertyName("date_of_expiration")]
    public DateTime? DateOfExpiration { get; set; }

    [JsonPropertyName("issuer_id")]
    public string? IssuerId { get; set; }

    [JsonPropertyName("payment_method_id")]
    public string PaymentMethodId { get; set; }

    [JsonPropertyName("payment_type_id")]
    public string PaymentTypeId { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("currency_id")]
    public string? CurrencyId { get; set; } = "BRL";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("live_mode")]
    public bool? LiveMode { get; set; } // TRUE = produção & FALSE = sandbox

    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; set; }

    [JsonPropertyName("shipping_amount")]
    public decimal? ShippingAmount { get; set; }

    [JsonPropertyName("payer")]
    public PayerRequest? PayerRequest { get; set; }

    [JsonPropertyName("additional_info")]
    public AdditionalInfoResponse? AdditionalInfo { get; set; }

    [JsonPropertyName("transaction_amount")]
    public decimal TransactionAmount { get; set; }

    [JsonPropertyName("statement_descriptor")]
    public string? StatementDescriptor { get; set; }

    [JsonPropertyName("installments")]
    public int? Installments { get; set; }

    [JsonPropertyName("status_detail")]
    public string StatusDetail { get; set; }

    [JsonPropertyName("external_reference")]
    public string? ExternalReference { get; set; }

    [JsonPropertyName("point_of_interaction")]
    public PointOfInteraction? PointOfInteraction { get; set; }

    [JsonPropertyName("barcode")]
    public BarcodeData? Barcode { get; set; }
}

public class PointOfInteraction
{
    [JsonPropertyName("transaction_data")]
    public TransactionData? TransactionData { get; set; }
}

public class TransactionData
{
    [JsonPropertyName("qr_code")]
    public string? QrCode { get; set; }

    [JsonPropertyName("qr_code_base64")]
    public string? QrCodeBase64 { get; set; }

    [JsonPropertyName("ticket_url")]
    public string? TicketUrl { get; set; }
}

public class BarcodeData
{
    [JsonPropertyName("content")]
    public string? Content { get; set; }
}
