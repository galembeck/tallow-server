using Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Repository.Configurations;

namespace Repository;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    #region ENTITIES

    public DbSet<User> Users { get; set; }
    public DbSet<UserSecurityInfo> UserSecurityInfos { get; set; }
    public DbSet<UserHistoric> UserHistorics { get; set; }
    public DbSet<AccessToken> AccessTokens { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Product> Products { get; set; }


    #endregion ENTITIES

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Model.SetMaxIdentifierLength(30);

        modelBuilder.Model.ToDebugString();
    }
}
