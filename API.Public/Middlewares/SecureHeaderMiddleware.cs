using OwaspHeaders.Core.Enums;
using OwaspHeaders.Core.Extensions;
using OwaspHeaders.Core.Models;

namespace API.Public.Middlewares;

public static class SecureHeaderMiddleware
{
    public static SecureHeadersMiddlewareConfiguration GetConfigs()
    {
        return SecureHeadersMiddlewareBuilder
            .CreateBuilder()
            .UseHsts(1200, false)
            .UseXssProtection()//(XssMode.oneReport, "https://reporturi.com/some-report-url
            .UsePermittedCrossDomainPolicies(XPermittedCrossDomainOptionValue.masterOnly)
            .UseReferrerPolicy(ReferrerPolicyOptions.sameOrigin)
            .Build();
    }
}
