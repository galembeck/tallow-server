using Domain.Repository;
using Domain.Repository.User;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Repository.Repository;
using Repository.Repository.User;
using IWishlistRepository = Domain.Repository.IWishlistRepository;
using WishlistRepository = Repository.Repository.WishlistRepository;

namespace IoC;

public static class NativeInjector
{
    public static void ConfigureInjections(this IServiceCollection services)
    {
        #region .: INTERNAL INJECTIONS :.

        #region .: USER :.
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        services.AddScoped<IUserAddressService, UserAddressService>();

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

        #region .: ORDER :.

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderService, OrderService>();

        #endregion .: ORDER :.

        #region .: WISHLIST :.

        services.AddScoped<IWishlistRepository, WishlistRepository>();
        services.AddScoped<IWishlistService, WishlistService>();

        #endregion .: WISHLIST :.

        #region .: COUPON :.

        services.AddScoped<ICouponRepository, CouponRepository>();
        services.AddScoped<ICouponService, CouponService>();

        #endregion .: COUPON :.

        #region .: GIFT CARD :.

        services.AddScoped<IGiftCardRepository, GiftCardRepository>();
        services.AddScoped<IGiftCardService, GiftCardService>();

        #endregion .: GIFT CARD :.

        #region .: PAYMENT :.

        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentService, PaymentService>();

         services.AddScoped<IMercadoPagoService, MercadoPagoService>();

        #endregion .: PAYMENT :.

        #region .: EMAIL :.

        services.AddScoped<IEmailService, EmailService>();

        #endregion .: EMAIL :.

        #region .: JOBS :.

        services.AddScoped<ITrackingCodeEmailJob, TrackingCodeEmailJob>();

        #endregion .: JOBS :.

        #endregion .: INTERNAL INJECTIONS :.
    }
}
