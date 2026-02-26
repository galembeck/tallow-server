namespace Domain.Utils;

public static class EnvType
{
    public static string Development { get; }
    public static string Production { get; }

    static EnvType()
    {
        Development = "Development";
        Production = "Production";
    }
}
