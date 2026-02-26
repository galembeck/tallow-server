using API.Public.Resources;
using AspNetCoreRateLimit;
using Azure.Core;
using Domain.Constants;
using Domain.Data.Entities;
using Domain.Data.Models.Auth;
using Domain.Data.Models.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Net.NetworkInformation;
using UAParser;

namespace API.Public.Controllers._Base;

[ApiController]
[ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
public class _BaseController : ControllerBase
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public _BaseController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public _BaseController()
    {
    }

    public static IdentityPrincipal? Authenticated
        => Thread.CurrentPrincipal as IdentityPrincipal;

    protected UserSecurityInfo? GetSecurityInfo(HttpRequest request)
    {
        if (request != null)
        {
            IEnumerable<string> headerValues = Request.Headers["X-Forwarded-For"];
            var ip = headerValues?.FirstOrDefault()?.Split(',')?[0] ?? "";

            var nics = NetworkInterface.GetAllNetworkInterfaces();
            var macAddress = string.Empty;
            foreach (var adapter in nics)
            {
                if (macAddress != string.Empty)
                    continue;

                var properties = adapter.GetIPProperties();
                macAddress = adapter.GetPhysicalAddress().ToString();
            }

            var userAgent = request.Headers[HeaderNames.UserAgent];
            var uaParser = Parser.GetDefault();
            var c = uaParser.Parse(userAgent);
            var browser = c.UA.Family + " " + c.UA.Major + "." + c.UA.Minor + " " + c.OS.Family;

            var securityInfo = new UserSecurityInfo
            {
                Ip = ip,
                MacAdress = macAddress,
                Browser = browser,
                Header = Request.Headers.ToList(),
            };

            return securityInfo;
        }
        else
        {
            return null;
        }
    }

    protected void GenerateAuthCookie(Tokens model)
    {
        if (_httpContextAccessor?.HttpContext?.Response?.Cookies != null)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                "AccessToken",
                model.AccessToken,
                new CookieOptions
                {
                    Expires = model.AccessTokenExpiresAt,
                    HttpOnly = true,
                    Secure = true,
                    Domain = Constant.Settings.Domain
                }
            );

            _httpContextAccessor.HttpContext.Response.Cookies.Append(
                "RefreshToken",
                model.RefreshToken,
                new CookieOptions
                {
                    Expires = model.RefreshTokenExpiresAt,
                    HttpOnly = true,
                    Secure = true,
                    Domain = Constant.Settings.Domain
                }
            );
        }
    }

    protected void RemoveAuthCookie(Tokens model)
    {
        if (_httpContextAccessor?.HttpContext?.Response?.Cookies != null)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("AccessToken");
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("RefreshToken");
        }
    }
}