namespace Domain.Utils.Constants;

public sealed record PaymentServiceSettings
{
    public string ServiceName { get; set; }
    public string Endpoint { get; set; }
    public string AccessToken { get; set; }
    public string PublicKey { get; set; }
}
