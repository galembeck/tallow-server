using API.Public.Middlewares;
using OwaspHeaders.Core.Extensions;

namespace API.Public.Configuration;

public static class HandlersInitializer
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
    }

    public static void ConfigureSecureHeadersHandler(this IApplicationBuilder app)
    {
        app.UseSecureHeadersMiddleware(SecureHeaderMiddleware.GetConfigs());
    }
}
