using Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Configuration;
using Repository.Configurations;

namespace Repository;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    #region .: ENTITIES :.

    public DbSet<User> Users { get; set; }
    public DbSet<UserAddress> UserAddresses { get; set; }
    public DbSet<UserSecurityInfo> UserSecurityInfos { get; set; }
    public DbSet<UserHistoric> UserHistorics { get; set; }
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<GiftCard> GiftCards { get; set; }

    #endregion .: ENTITIES :.

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        #region .: CONFIGURATION :.

        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentConfiguration());

        #endregion .: CONFIGURATION :.

        modelBuilder.Model.SetMaxIdentifierLength(30);

        modelBuilder.Model.ToDebugString();
    }
}
