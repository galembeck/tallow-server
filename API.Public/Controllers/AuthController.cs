using API.Public.Controllers._Base;
using API.Public.DTOs;
using API.Public.DTOs.Auth;
using API.Public.Filters;
using API.Public.Validators;
using Domain.Enumerators;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Exceptions;

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
            throw new System.Security.Authentication.AuthenticationException(AuthenticationErrorMessage.UNAUTHORIZED.ToString());

        var model = await _authService.RevokeAccessTokenAsync(accessToken, refreshToken, Authenticated.User);

        GenerateAuthCookie(model);

        return Ok();
    }

    // ─────────────────────────────────────────────────────────────────────────
    //  PASSWORD RECOVERY
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Step 1 — Request a 6-character OTP sent to the user's e-mail.
    /// Always returns 200 to prevent e-mail enumeration.
    /// </summary>
    [HttpPost("recovery/request")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RequestPasswordRecovery(
        [FromBody] PasswordRecoveryRequestDTO body,
        CancellationToken cancellationToken = default)
    {
        await _authService.SendPasswordRecoveryAsync(body.Email, cancellationToken);
        return Ok(new { message = "Se o e-mail existir em nossa base, você receberá um código de recuperação em instantes." });
    }

    /// <summary>
    /// Step 2 — Verify the OTP token before showing the new-password form.
    /// </summary>
    [HttpPost("recovery/verify")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyRecoveryToken(
        [FromBody] PasswordRecoveryVerifyDTO body,
        CancellationToken cancellationToken = default)
    {
        var valid = await _authService.VerifyPasswordRecoveryTokenAsync(body.Email, body.Token, cancellationToken);

        if (!valid)
            return BadRequest(new { message = "Código inválido ou expirado." });

        return Ok(new { message = "Código verificado com sucesso." });
    }

    /// <summary>
    /// Step 3 — Reset the password. Token is validated again server-side before the update.
    /// </summary>
    [HttpPost("recovery/reset")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] PasswordResetDTO body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _authService.ResetPasswordAsync(body.Email, body.Token, body.NewPassword, cancellationToken);
            return Ok(new { message = "Senha redefinida com sucesso. Faça login com sua nova senha." });
        }
        catch (BusinessException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
