namespace Domain.Utils.Constants;

public sealed record ShippingServiceSettings
{
    public string ShippingServiceName { get; set; }
    public string ServiceShippingEndpoint { get; set; }
    public string ServiceAPIKey { get; set; }
    public string ShippingPostalCode { get; set; }
}