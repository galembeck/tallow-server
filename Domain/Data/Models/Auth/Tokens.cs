namespace Domain.Data.Models.Auth;

public class Tokens
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset? AccessTokenExpiresAt { get; set; }

    public string RefreshToken { get; set; } = string.Empty;
    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }
}
