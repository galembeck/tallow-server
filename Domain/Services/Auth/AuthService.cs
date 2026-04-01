using Domain.Constants;
using Domain.Data.Entities;
using Domain.Data.Models.Auth;
using Domain.Data.Models.DTOs;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Repository.User;
using Domain.Utils;
using Hangfire;

namespace Domain.Services;

public class AuthService : IAuthService
{
    private readonly IAccessTokenRepository _accessTokenRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserSecurityInfoRepository _userSecurityInfoRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;

    private readonly IUserService _userService;

    public AuthService(
        IUserRepository userRepository,
        IAccessTokenRepository accessTokenRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUserSecurityInfoRepository userSecurityInfoRepository,
        IBackgroundJobClient backgroundJobClient,
        IUserService userService)
    {
        _userRepository = userRepository;
        _accessTokenRepository = accessTokenRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _userSecurityInfoRepository = userSecurityInfoRepository;
        _backgroundJobClient = backgroundJobClient;

        _userService = userService;
    }

    public async Task<Tokens> AuthenticateAsync(string email, string password, UserSecurityInfo securityInfo)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user is null)
            throw new BusinessException(BusinessErrorMessage.USER_NOT_FOUND);

        if (user.Password != StringUtil.SHA512(password))
            throw new BusinessException(BusinessErrorMessage.USER_NOT_FOUND_OR_INVALID_PASSWORD);

        var response = await GenerateTokensAsync(user.Id);

        user.LastAccessAt = DateTimeOffset.UtcNow;

        await _userRepository.UpdateAsync(user);

        securityInfo.UserId = user.Id;
        securityInfo.Moment = SecurityInfoMoment.LOGIN;

        await _userSecurityInfoRepository.InsertAsync(securityInfo.WithoutRelations(securityInfo));

        return response;
    }

    public async Task<Tokens> RefreshAsync(string refreshTokenId)
    {
        RefreshToken refresh = await _refreshTokenRepository.GetAsync(refreshTokenId);
        if (refresh is null) { throw new AuthenticationException(AuthenticationErrorMessage.UNAUTHORIZED); }

        User user = await _userRepository.GetAsync(refresh.UserId);
        if (user is null) { throw new AuthenticationException(AuthenticationErrorMessage.UNAUTHORIZED); }

        if (refresh.ExpiresAt < DateTimeOffset.UtcNow) { throw new AuthenticationException(AuthenticationErrorMessage.TOKEN_EXPIRED); }

        Tokens response = await GenerateTokensAsync(refresh.UserId);

        user.LastAccessAt = DateTimeOffset.UtcNow;
        await _userRepository.UpdateAsync(user);

        return response;
    }

    public async Task<Tokens> RevokeAccessTokenAsync(string accessTokenId, string refreshTokenId, User actor)
    {
        AccessTokenDTO accessTokenDto = await _accessTokenRepository.GetByToken(accessTokenId);
        RefreshToken refreshToken = await _refreshTokenRepository.GetAsync(refreshTokenId);

        if (accessTokenDto != null)
        {
            accessTokenDto.UpdatedBy = actor.Id;
            accessTokenDto.DeletedAt = DateTimeOffset.UtcNow;
            accessTokenDto.ExpiresAt = DateTimeOffset.MinValue;

            _ = await _accessTokenRepository.UpdatePartialAsync(
                new AccessToken
                {
                    Id = accessTokenDto.Id
                },
                accessToken =>
                {
                    accessToken.UpdatedBy = accessTokenDto.UpdatedBy;
                    accessToken.DeletedAt = accessTokenDto.DeletedAt;
                    accessToken.ExpiresAt = accessTokenDto.ExpiresAt;
                },
                actor.Id);
        }
        else
            throw new AuthenticationException(AuthenticationErrorMessage.ACCESSTOKEN_NOT_FOUND);

        if (refreshToken != null)
        {
            refreshToken.UpdatedBy = actor.Id;
            refreshToken.DeletedAt = DateTimeOffset.UtcNow;
            refreshToken.ExpiresAt = DateTimeOffset.MinValue;

            refreshToken = await _refreshTokenRepository.UpdateAsync(refreshToken, actor.Id);
        }
        else
        {
            throw new AuthenticationException(AuthenticationErrorMessage.REFRESHTOKEN_NOT_FOUND);
        }

        return new Tokens
        {
            AccessToken = accessTokenDto.Id,
            AccessTokenExpiresAt = accessTokenDto.ExpiresAt,
            RefreshToken = refreshToken.Id,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
        };
    }

    //public async Task SendPasswordRecoveryAsync(SendTo? sendTo, string document)
    //{
    //    User userSaved = await _userRepository.GetByDocumentAsync(document);

    //    if (userSaved != null)
    //    {
    //        string token = StringUtil.GenerateRandom(Constant.Settings.AuthSettings.RecoveryPasswordLength).ToUpper();
    //        DateTimeOffset expiresAt = DateTimeOffset.UtcNow.AddMinutes(Constant.Settings.AuthSettings.RecoveryPasswordExpiration);

    //        switch (sendTo)
    //        {
    //            case SendTo.EMAIL:
    //                var logMessage = "AuthService.SendPasswordRecoveryAsync() | Sending to: " + userSaved?.Email + " | Token: " + token ?? null;
    //                logMessage = StringUtil.SanitizeTokenLog(logMessage);

    //                try
    //                {
    //                    await Mailer.SendRecoveryPasswordAsync(userSaved.Email, token, userSaved.Name, expiresAt);

    //                    await _logService.CreateLogsAsync(
    //                       Integration.MARKETPLACE,
    //                       Operation.EMAIL_SENDER,
    //                       logMessage,
    //                       null,
    //                       Status.SUCCESS,
    //                       userSaved?.Id,
    //                       null,
    //                       null,
    //                       null,
    //                       EnumLogType.EMAIL_SENDER,
    //                       false);

    //                    break;
    //                }
    //                catch (System.Exception ex)
    //                {
    //                    await _logService.CreateLogsAsync(
    //                        Integration.MARKETPLACE,
    //                        Operation.EMAIL_SENDER,
    //                        logMessage,
    //                        ex?.Message,
    //                        Status.ERROR,
    //                        userSaved?.Id,
    //                        null,
    //                        null,
    //                        null,
    //                        EnumLogType.EMAIL_SENDER,
    //                        false);

    //                    throw new BusinessException(BusinessErrorMessage.VPN_ERROR);
    //                }

    //            case SendTo.CELLPHONE:
    //                try
    //                {
    //                    await _gmpService.SendSMS(userSaved.Cellphone, token, userSaved.UserOrigin);
    //                }
    //                catch (System.Exception ex)
    //                {
    //                    logMessage = "AuthService.SendPasswordRecoveryAsync() | Sending to: " + userSaved?.Cellphone + " | Token: " + token ?? null;
    //                    logMessage = StringUtil.SanitizeTokenLog(logMessage);

    //                    await _logService.CreateLogsAsync(
    //                        Integration.MARKETPLACE,
    //                        Operation.GMP_SEND_SMS,
    //                        logMessage,
    //                        ex?.Message,
    //                        Status.ERROR,
    //                        userSaved?.Id,
    //                        null,
    //                        null,
    //                        null,
    //                        EnumLogType.APPLICATION,
    //                        false);

    //                    throw new BusinessException(BusinessErrorMessage.VPN_ERROR);
    //                }
    //                break;

    //            default:
    //                break;
    //        }

    //        userSaved.PasswordChangeToken = token;
    //        userSaved.PasswordChangeTokenExpiresAt = expiresAt;

    //        await _userRepository.UpdateAsync(userSaved);
    //    }
    //}

    //public async Task RecoverPasswordAsync(string document, string changeToken, string password, UserOrigin userOrigin)
    //{
    //    User user = await _userRepository.GetByDocumentAndTokenAsync(document, changeToken, userOrigin);

    //    if (user is null)
    //        throw new BusinessException(BusinessErrorMessage.INVALID_DOCUMENT_OR_RECOVERY_PASSWORD_TOKEN);

    //    user.PasswordChangeToken = null;
    //    user.PasswordChangeTokenExpiresAt = null;
    //    user.Password = StringUtil.SHA512(password);

    //    await _userRepository.UpdateAsync(user);
    //}

    //public async Task<List<User>> ValidateGoogleTokenAsync(string idToken, UserOrigin origin, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        var email = await _googleMechanism.ValidateGoogleAccessTokenAsync(idToken);

    //        if (string.IsNullOrEmpty(email))
    //            throw new AuthenticationException(AuthenticationErrorMessage.INVALID_TOKEN);

    //        var users = await _userService.GetUsersByEmail(email, origin);

    //        return users;
    //    }
    //    catch (System.Exception ex)
    //    {
    //        await _logService.CreateLogsAsync(
    //            Integration.MARKETPLACE,
    //            Operation.VALIDATE_TOKEN_GOOGLE,
    //            "ValidateGoogleTokenAsync",
    //            ex.Message,
    //            Status.ERROR,
    //            null,
    //            null,
    //            null,
    //            null,
    //            EnumLogType.APPLICATION);

    //        return null;
    //    }
    //}

    //public async Task<List<User>> ValidateFacebookTokenAsync(string idToken, UserOrigin origin, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        string email = await _facebookMechanism.VerifyFacebookTokenAndGetEmail(idToken, origin);

    //        if (string.IsNullOrEmpty(email))
    //            throw new AuthenticationException(AuthenticationErrorMessage.INVALID_TOKEN);

    //        var users = await _userService.GetUsersByEmail(email, origin);

    //        return users;
    //    }
    //    catch (System.Exception ex)
    //    {
    //        await _logService.CreateLogsAsync(
    //            Integration.MARKETPLACE,
    //            Operation.VALIDATE_TOKEN_FACEBOOK,
    //            "ValidateFacebookTokenAsync",
    //            ex.Message,
    //            Status.ERROR,
    //            null,
    //            null,
    //            null,
    //            null,
    //            EnumLogType.APPLICATION);

    //        return null;
    //    }
    //}

    //public async Task<Tokens> SocialAuthenticate(string primaryDocument, string email, string redeFacilCode, UserOrigin origin, UserSecurityInfo userSecurityInfo)
    //{
    //    try
    //    {
    //        User user = await _userService.GetByDocumentAndEmailAsync(primaryDocument, email, origin);

    //        if (user is null)
    //            throw new BusinessException(BusinessErrorMessage.USER_NOT_FOUND);

    //        var response = await GenerateTokensAsync(user.Id);

    //        user = await _userService.VerifyInstallation(user);

    //        user.LastAccessAt = DateTimeOffset.UtcNow;
    //        await _userRepository.UpdateAsync(user);

    //        // Save security information
    //        userSecurityInfo.UserId = user.Id;
    //        userSecurityInfo.Moment = SecurityInfoMoment.LOGIN;
    //        await _userSecurityInfoRepository.InsertAsync(userSecurityInfo.WithoutRelations(userSecurityInfo));

    //        if (!string.IsNullOrEmpty(redeFacilCode))
    //        {
    //            RedeFacilAccesses redeFacilAccesses = new();

    //            redeFacilAccesses.RedeFacilCode = redeFacilCode;
    //            redeFacilAccesses.Login = true;

    //            await _redeFacilRepository.InsertAsync(redeFacilAccesses);
    //        }

    //        return response;
    //    }
    //    catch (System.Exception ex)
    //    {
    //        await _logService.CreateLogsAsync(
    //            Integration.MARKETPLACE,
    //            Operation.SOCIAL_AUTHENTICATON,
    //            "SocialAuthenticate",
    //            ex.Message,
    //            Status.ERROR,
    //            null,
    //            null,
    //            null,
    //            null,
    //            EnumLogType.APPLICATION);

    //        throw;
    //    }
    //}



    #region .: PASSWORD RECOVERY :.

    public async Task SendPasswordRecoveryAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        // Always succeed silently to prevent e-mail enumeration
        if (user is null)
            return;

        var token     = StringUtil.GenerateRandom(Constant.Settings.AuthSettings.RecoveryPasswordLength).ToUpper();
        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(Constant.Settings.AuthSettings.RecoveryPasswordExpiration);

        await _userRepository.UpdatePartialAsync(
            new User { Id = user.Id },
            u =>
            {
                u.PasswordChangeToken          = token;
                u.PasswordChangeTokenExpiresAt = expiresAt;
            });

        _backgroundJobClient.Enqueue<IEmailService>(s =>
            s.SendPasswordRecoveryEmailAsync(user.Name, user.Email, token, expiresAt.UtcDateTime));
    }

    public async Task<bool> VerifyPasswordRecoveryTokenAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAndTokenAsync(email, token, cancellationToken);
        return user is not null;
    }

    public async Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAndTokenAsync(email, token, cancellationToken);

        if (user is null)
            throw new BusinessException(BusinessErrorMessage.INVALID_DOCUMENT_OR_RECOVERY_PASSWORD_TOKEN);

        await _userRepository.UpdatePartialAsync(
            new User { Id = user.Id },
            u =>
            {
                u.Password                     = StringUtil.SHA512(newPassword);
                u.PasswordChangeToken          = null;
                u.PasswordChangeTokenExpiresAt = null;
            });
    }

    #endregion .: PASSWORD RECOVERY :.

    #region .: PRIVATE METHODS :.

    private async Task<Tokens> GenerateTokensAsync(string userId)
    {
        AccessToken accessToken = await _accessTokenRepository.InsertAsync(new AccessToken()
        {
            UserId = userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(Constant.Settings.AuthSettings.AccessTokenExpiration),
        });

        RefreshToken refreshToken = await _refreshTokenRepository.InsertAsync(new RefreshToken()
        {
            UserId = userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(Constant.Settings.AuthSettings.RefreshTokenExpiration),
        });

        return new Tokens
        {
            AccessToken = accessToken.Id,
            AccessTokenExpiresAt = accessToken.ExpiresAt,
            RefreshToken = refreshToken.Id,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt,
        };
    }

    #endregion .: PRIVATE METHODS :.
}
