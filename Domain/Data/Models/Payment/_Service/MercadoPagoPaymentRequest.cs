using System.Text.Json.Serialization;

namespace Domain.Data.Models;

public class MercadoPagoPaymentRequest
{
    [JsonPropertyName("token")]
    public string? Token { get; set; }

    [JsonPropertyName("transaction_amount")]
    public decimal TransactionAmount { get; set; }

    [JsonPropertyName("statement_descriptor")]
    public string? StatementDescriptor { get; set; }

    [JsonPropertyName("payment_method_id")]
    public string PaymentMethodId { get; set; }

    [JsonPropertyName("payer")]
    public PayerRequest Payer { get; set; }

    [JsonPropertyName("notification_url")]
    public string? NotificationUrl { get; set; }

    /// <summary>
    /// Dados do dispositivo (para análise de fraude)
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }

    [JsonPropertyName("issuer_id")]
    public string? IssuerId { get; set; }

    [JsonPropertyName("installments")]
    public int Installments { get; set; }

    [JsonPropertyName("external_reference")]
    public string? ExternalReference { get; set; }

    [JsonPropertyName("date_of_expiration")]
    public DateTime? DateOfExpiration { get; set; }

    [JsonPropertyName("differential_pricing_id")]
    public string? DifferentialPricingId { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("coupon_code")]
    public string? CouponCode { get; set; }

    [JsonPropertyName("coupon_amount")]
    public decimal? CouponAmount { get; set; }

    /// <summary>
    /// Captura automática do pagamento (true por padrão)
    /// </summary>
    [JsonPropertyName("capture")]
    public bool? Capture { get; set; }

    [JsonPropertyName("campaign_id")]
    public string? CampaignId { get; set; }

    [JsonPropertyName("callback_url")]
    public string? CallbackUrl { get; set; }

    /// <summary>
    /// TRUE: pagamentos aprovados OU reprovados / FALSE: in_process, também
    /// </summary>
    [JsonPropertyName("binary_mode")]
    public bool? BinaryMode { get; set; }

    //[JsonPropertyName("application_fee")]
    //public decimal ApplicationFee { get; set; }

    /// <summary>
    /// Informações adicionais sobre o pagamento
    /// </summary>
    [JsonPropertyName("additional_info")]
    public AdditionalInfoRequest? AdditionalInfo { get; set; }
}