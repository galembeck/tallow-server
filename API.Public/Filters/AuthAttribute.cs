using API.Public.Resources;
using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Public.Filters;

public class AuthAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var req = httpContext.Request;

        var token = req.Cookies["AccessToken"];

        if (string.IsNullOrEmpty(token))
            throw new AuthenticationException(AuthenticationErrorMessage.UNAUTHORIZED);

        IAccessTokenRepository accessTokenRepository = httpContext.RequestServices
            .GetRequiredService<IAccessTokenRepository>()
            ?? throw new ArgumentNullException(nameof(accessTokenRepository), "IAccessTokenRepository service is not registered in the dependency injection container");

        IUserRepository userRepository = httpContext.RequestServices
            .GetRequiredService<IUserRepository>()
            ?? throw new ArgumentNullException(nameof(userRepository), "IUserRepository service is not registered in the dependency injection container");

        AccessToken? accessToken;

        try
        {
            accessToken = await accessTokenRepository.GetAsync(token);
        }
        catch (PersistenceException)
        {
            throw new AuthenticationException(AuthenticationErrorMessage.UNAUTHORIZED);
        }

        if (accessToken is null)
            throw new AuthenticationException(AuthenticationErrorMessage.UNAUTHORIZED);

        if (accessToken.ExpiresAt < DateTimeOffset.UtcNow)
            throw new AuthenticationException(AuthenticationErrorMessage.TOKEN_EXPIRED);

        User? user = await userRepository.GetAsync(accessToken.UserId);

        if (user is null)
            throw new AuthenticationException(AuthenticationErrorMessage.UNAUTHORIZED);
        else
            IdentityResources.AddUserOnThread(user);

        await next();
    }
}