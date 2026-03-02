namespace Domain.Utils.Constants;

public sealed record Settings
{
    public string BaseUrl { get; set; }

    public string Version { get; init; } = string.Empty;
    public string Environment { get; init; } = string.Empty;
    public string Domain { get; init; } = string.Empty;
    public string SystemId { get; init; } = string.Empty;
    public int MaxPoolConnections { get; set; }

    public AuthSettings AuthSettings { get; set; } = new AuthSettings();
    public JwtSettings JwtSettings { get; set; } = new JwtSettings();

    public ShippingServiceSettings ShippingServiceSettings { get; set; } = new ShippingServiceSettings();

    public string FintechPass { get; set; } = string.Empty;
    public string CriptBankKey { get; set; } = string.Empty;
}
