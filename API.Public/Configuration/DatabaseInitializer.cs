using Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace API.Public.Configuration;

public static class DatabaseInitializer
{
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = Constant.CoreContextConnectionString;
        services.AddDbContextPool<AppDbContext>(options =>
            options.UseSqlServer(Constant.CoreContextConnectionString, b => b.MigrationsAssembly("Repository")),
                Constant.Settings.MaxPoolConnections
            );


        //builder.Services.AddDbContext<AppDbContext>(options =>
        //{
        //    options.UseSqlServer(builder.Configuration.GetConnectionString("CoreContextConnection"),
        //        b => b.MigrationsAssembly("Repository"));
        //});
    }
}