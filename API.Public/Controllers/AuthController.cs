using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.DTOs.Auth;
using API.Public.Filters;
using API.Public.Validators;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace API.Public.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : _BaseController
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService,
        IUserService userService,
        IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _authService = authService ?? throw new ArgumentNullException();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Authenticate([FromBody] AuthenticateDTO body)
    {
        var securityInfo = base.GetSecurityInfo(Request);

        await new AuthenticateValidator().ValidateAndThrowAsync(body);

        var model = await _authService
            .AuthenticateAsync(body.Email, body.Password, securityInfo);

        return Ok(AuthResponseDTO.ModelToDTO(model));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO body)
    {
        var model = await _authService.RefreshAsync(body.RefreshToken);

        GenerateAuthCookie(model);

        return Ok(AuthResponseDTO.ModelToDTO(model));
    }

    [AuthAttribute]
    [Filters.Authorize(ProfileType.CLIENT, ProfileType.ADMIN)]
    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeAccessToken()
    {
        var accessToken = Request.Cookies["AccessToken"];
        var refreshToken = Request.Cookies["RefreshToken"];

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            throw new AuthenticationException(AuthenticationErrorMessage.UNAUTHORIZED.ToString());

        var model = await _authService.RevokeAccessTokenAsync(accessToken, refreshToken, Authenticated.User);

        GenerateAuthCookie(model);

        return Ok();
    }
}
