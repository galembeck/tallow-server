using Domain.Constants;
using Domain.Utils.Constants;

namespace API.Public.Configuration;

public static class EnvironmentInitializer
{
    public static void ConfigureEnvironment(this IConfiguration configuration)
    {
        Constant.SetSettings(configuration.GetSection("Settings").Get<Settings>());
        Constant.SetCoreContextConnectionString(configuration.GetConnectionString("CoreContextConnection"));
    }

    public static async Task ConfigureEnvironmentAsync(this IConfiguration configuration)
    {
        Constant.SetSettings(configuration.GetSection("Settings").Get<Settings>());
    }
}
