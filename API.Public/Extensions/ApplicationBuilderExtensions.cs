using API.Public.Configuration;
using API.Public.Hubs;
using API.Public.Resources;
using AspNetCoreRateLimit;
using Domain.Constants;
using Domain.Utils;
using Scalar.AspNetCore;

namespace API.Public.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureApplication(this WebApplication app)
    {
        string currentEnv = app.Environment.EnvironmentName;

        Constant.SystemStartedAt = DateTimeOffset.UtcNow;
        Constant.ContentRootPath = app.Environment.ContentRootPath;

        Console.WriteLine($"Program: EnvironmentName: {currentEnv}");
        Console.WriteLine($"Program: SystemStartedAt: {Constant.SystemStartedAt}");
        Console.WriteLine($"Program: ContentRootPath: {Constant.ContentRootPath}");

        app.UseHsts();
        app.ConfigureExceptionHandler();
        app.UseStaticFiles();
        app.ConfigureSecureHeadersHandler();
        app.UseResponseCompression();
        app.UseCorrelationId();
        
        if (currentEnv != EnvType.Development)
        {
            app.UseHttpsRedirection();
        }

        app.MapOpenApi();
        app.MapScalarApiReference();

        app.UseIpRateLimiting();
        app.UseCookiePolicy();
        app.UseCors("_myAllowSpecificOrigins");
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<AdminNotificationHub>("/hubs/notifications");
        app.UseHangfireDashboard();
    }
 }
