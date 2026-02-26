namespace Domain.Utils.Constants;

public sealed record AuthSettings
{
    public int AccessTokenExpiration { get; set; }
    public int RefreshTokenExpiration { get; set; }

    public int RecoveryPasswordExpiration { get; set; }
    public int RecoveryPasswordLength { get; set; }
}
