using Domain.Utils.Constants;

namespace Domain.Constants;

public static class Constant
{
    public static string CoreContextConnectionString { get; private set; } = string.Empty;
    public static Settings Settings { get; private set; } = new Settings();
    public static DateTimeOffset SystemStartedAt { get; set; }
    public static string ContentRootPath { get; set; } = string.Empty;

    public static void SetSettings(Settings settings)
    {
        Settings = settings;
    }

    public static void SetCoreContextConnectionString(String connection)
    {
        CoreContextConnectionString = connection;
    }
}
