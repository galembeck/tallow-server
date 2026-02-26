using AspNetCoreRateLimit;

namespace API.Public.Configuration;
public static class RateLimitInitializer
{
    public static void ConfigureRateLimit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.AddMemoryCache();
        services.Configure<IpRateLimitOptions>(configuration.GetSection("Settings").GetSection("IpRateLimiting"));
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddHttpContextAccessor();
    }
}