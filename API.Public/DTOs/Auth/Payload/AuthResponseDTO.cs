using Domain.Data.Models.Auth;

namespace API.Public.DTOs.Auth;

public class AuthResponseDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }



    public AuthResponseDTO(Tokens o)
    {
        if (o == null) return;

        AccessToken = o.AccessToken;
        RefreshToken = o.RefreshToken;
    }

    public static AuthResponseDTO ModelToDTO(Tokens o)
    {
        return o is null ? null : new AuthResponseDTO(o);
    }
}
