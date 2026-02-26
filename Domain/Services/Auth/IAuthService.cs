using Domain.Data.Entities;
using Domain.Data.Models.Auth;
using Domain.Enumerators;

namespace Domain.Services;

public interface IAuthService
{
    Task<Tokens> AuthenticateAsync(string email, string password, UserSecurityInfo securityInfo);

    Task<Tokens> RefreshAsync(string refreshTokenId);

    Task<Tokens> RevokeAccessTokenAsync(string accessTokenId, string refreshTokenId, User actor);

    //Task SendPasswordRecoveryAsync(SendTo? sendTo, string document);

    //Task RecoverPasswordAsync(string document, string changeToken, string password);

    //Task<List<User>> ValidateGoogleTokenAsync(string idToken, CancellationToken cancellationToken = default);

    //Task<List<User>> ValidateFacebookTokenAsync(string idToken, CancellationToken cancellationToken = default);

    //Task<Tokens> SocialAuthenticate(string primaryDocument, string email, UserSecurityInfo userSecurityInfo);
}
