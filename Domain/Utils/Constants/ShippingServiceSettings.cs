namespace Domain.Utils.Constants;

public sealed record ShippingServiceSettings
{
    public string ShippingServiceName { get; set; }
    public string ServiceShippingEndpoint { get; set; }
    public string ServiceAPIKey { get; set; }
    public string ShippingPostalCode { get; set; }

    // Sender (store) information used when creating shipping labels
    public string ShippingFromName { get; set; }
    public string ShippingFromAddress { get; set; }
    public string ShippingFromNumber { get; set; }
    public string? ShippingFromComplement { get; set; }
    public string ShippingFromDistrict { get; set; }
    public string ShippingFromCity { get; set; }
    public string ShippingFromState { get; set; }   // 2-letter uppercase, e.g. "SP"
    public string? ShippingFromDocument { get; set; } // CPF or CNPJ of the store
}