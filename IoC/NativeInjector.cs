using Domain.Repository;
using Domain.Repository.User;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Repository.Repository;
using Repository.Repository.User;

namespace IoC;

public static class NativeInjector
{
    public static void ConfigureInjections(this IServiceCollection services)
    {
        #region .: INTERNAL INJECTIONS :.

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IUserSecurityInfoRepository, UserSecurityInfoRepository>();

        services.AddScoped<IUserHistoricRepository, UserHistoricRepository>();
        services.AddScoped<IUserHistoricService, UserHistoricService>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IAccessTokenRepository, AccessTokenRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        services.AddScoped<IFileStorageService, FileStorageService>();

        services.AddScoped<IShippingService, ShippingService>();

        #endregion .: INTERNAL INJECTIONS :.
    }
}
