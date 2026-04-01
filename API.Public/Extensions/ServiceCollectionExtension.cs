using API.Public.Configuration;
using API.Public.Services;
using AspNetCoreRateLimit;
using Domain.Constants;
using Domain.Services;
using Domain.Utils.Constants;
using IoC;
using Resend;

namespace API.Public.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

        services.ConfigureRateLimit(configuration);
        services.ConfigureDatabase(configuration);
        services.ConfigureJwt();
        //services.ConfigureLogger(configuration);
        services.AddCoreMemoryCache(configuration);

        services.AddHttpContextAccessor();
        services.AddHttpClient(Constant.Settings.ShippingServiceSettings.ShippingServiceName);

        services.AddHttpClient<IMercadoPagoService, MercadoPagoService>();

        services.ConfigureHangfire(configuration);
        services.ConfigureResend();
        services.ConfigureInjections();
        services.AddSignalR();
        services.AddScoped<IAdminNotificationService, AdminNotificationService>();
        services.AddAuthorization();
        services.AddResponseCompression();
        services.ConfigureCompression();
        services.ConfigureControllers();
        services.AddOpenApi();

        const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        services.AddCors(options =>
        {
            options.AddPolicy(MyAllowSpecificOrigins, policy =>
            {
                policy.WithOrigins(
                        "http://localhost:5173",
                        "http://localhost:5174")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.None;
            options.Secure = CookieSecurePolicy.Always;
        });
    }

    private static void ConfigureResend(this IServiceCollection services)
    {
        services.AddOptions();
        services.AddHttpClient<ResendClient>();
        services.Configure<ResendClientOptions>(o =>
        {
            o.ApiToken = Constant.Settings.EmailServiceSettings.ApiToken;
        });
        services.AddTransient<IResend, ResendClient>();
    }
}
