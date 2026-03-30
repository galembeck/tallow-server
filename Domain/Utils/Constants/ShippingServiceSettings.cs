namespace Domain.Utils.Constants;

public sealed record ShippingServiceSettings
{
    public string ShippingServiceName { get; set; }
    public string ServiceShippingEndpoint { get; set; }
    public string ServiceAPIKey { get; set; }
    public string ShippingPostalCode { get; set; }

    // Sender info used when generating shipping labels
    public string SenderName { get; set; }
    public string SenderDocument { get; set; }
    public string SenderEmail { get; set; }
    public string SenderPhone { get; set; }
    public string SenderAddress { get; set; }
    public string SenderAddressNumber { get; set; }
    public string SenderDistrict { get; set; }
    public string SenderCity { get; set; }
    public string SenderStateAbbr { get; set; }
}