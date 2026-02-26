using Microsoft.Extensions.Configuration;

namespace IoC;

public class MemoryCacheConfiguration
{
    public MemoryCacheConfiguration()
    {
    }

    public MemoryCacheConfiguration(IConfiguration configuration)
    {
        var settings = configuration.GetSection("MemoryCacheConfig").Get<MemoryCacheConfiguration>();
        if (settings != null)
        {
            CompactionPercentage = settings.CompactionPercentage > 0 ? settings.CompactionPercentage : 0;
            ExpirationMinutes = settings.ExpirationMinutes > 0 ? settings.ExpirationMinutes : 0;
            ExpirationScanFrequency = settings.ExpirationScanFrequency > 0 ? settings.ExpirationScanFrequency : 30;
        }
        else
        {
            throw new NotSupportedException("MemoryCacheSettings in appsettings is Needed.");
        }
    }

    /// <summary>
    /// Total minutes to expire.
    /// </summary>
    public int ExpirationMinutes { get; set; }

    /// <summary>
    /// Percent in Decimal case to compact memory, if size limit is over.
    /// </summary>
    public double CompactionPercentage { get; set; }

    /// <summary>
    /// Minimum length of time between successive scans for expired
    /// </summary>
    public int ExpirationScanFrequency { get; set; }

    public TimeSpan ExpirationScanFrequencyTime => TimeSpan.FromMinutes(ExpirationScanFrequency > 0 ? ExpirationScanFrequency : 30);
}
