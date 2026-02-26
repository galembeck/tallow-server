using Domain.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;

namespace IoC;

public static class  LoggerInitializer
{
    public static void ConfigureLogger(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("MachineName", Environment.MachineName)
            .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
            .Enrich.FromLogContext()
            .ConfigureMinimumLevel(configuration)
            .CanWriteToConsole()
            .CreateLogger();
    }

    /// <summary>
    /// Verbose = 0
    ///     Anything and everything you might want to know about a running block of code.
    /// Debug = 1
    ///     Internal system events that aren't necessarily observable from the outside.
    /// Information 2
    ///     The lifeblood of operational intelligence - things happen.
    /// Warning 3
    ///     Service is degraded or endangered.
    /// Error 4
    ///     Functionality is unavailable, invariants are broken or data is lost.
    /// Fatal 5
    /// If you have a pager, it goes off when one of these occurs.
    /// </summary>
    /// <param name="loggerConfig"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    private static LoggerConfiguration ConfigureMinimumLevel(this LoggerConfiguration loggerConfig, IConfiguration configuration)
    {
        if (configuration["Environment"] == EnvType.Development) { loggerConfig = loggerConfig.MinimumLevel.Information(); }
        if (configuration["Environment"] == EnvType.Production) { loggerConfig = loggerConfig.MinimumLevel.Error(); }

        return loggerConfig;
    }

    private static LoggerConfiguration CanWriteToConsole(this LoggerConfiguration loggerConfig)
    {
        if (Debugger.IsAttached)
            loggerConfig = loggerConfig.WriteTo.Console();

        return loggerConfig;
    }
}