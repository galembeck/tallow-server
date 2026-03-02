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

        #region .: USER :.
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        #endregion .: USER :.

        #region .: USER SECURITY INFO :.

        services.AddScoped<IUserSecurityInfoRepository, UserSecurityInfoRepository>();

        #endregion .: USER SECURITY INFO :.

        #region .: USER HISTORIC :.

        services.AddScoped<IUserHistoricRepository, UserHistoricRepository>();
        services.AddScoped<IUserHistoricService, UserHistoricService>();

        #endregion .: USER HISTORIC :.

        #region .: AUTH :.

        services.AddScoped<IAuthService, AuthService>();

        #endregion .: AUTH :.

        #region .: TOKENS :.

        services.AddScoped<IAccessTokenRepository, AccessTokenRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        #endregion .: TOKENS :.

        #region .: PRODUCT :.

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IProductService, ProductService>();

        #endregion .: PRODUCT :.

        #region .: FILE STORAGE :.

        services.AddScoped<IFileStorageService, FileStorageService>();

        #endregion .: FILE STORAGE :.

        #region .: SHIPPING :.

        services.AddScoped<IShippingService, ShippingService>();

        #endregion .: SHIPPING :.

        #region .: CART :.

        services.AddScoped<ICartRepository, CartRepository>();
        services.AddScoped<ICartService, CartService>();
        
        services.AddScoped<ICartItemRepository, CartItemRepository>();

        #endregion .: CART :.

        #endregion .: INTERNAL INJECTIONS :.
    }
}
