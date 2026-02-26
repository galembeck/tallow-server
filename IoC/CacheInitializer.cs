using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IoC;

public static class Initializer
{
    public static IServiceCollection AddCoreMemoryCache(this IServiceCollection services, IConfiguration configuration)
    {
        var configurationMemory = new MemoryCacheConfiguration(configuration);

        services.AddMemoryCache(option =>
        {
            option.CompactionPercentage = configurationMemory.CompactionPercentage;
            option.ExpirationScanFrequency = configurationMemory.ExpirationScanFrequencyTime;
        });

        services.Configure<MemoryCacheConfiguration>(options => configuration.GetSection("MemoryCacheConfig").Bind(options));

        return services;
    }

}
