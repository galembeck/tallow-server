using Domain.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repository.Configuration;

public class GiftCardConfiguration : IEntityTypeConfiguration<GiftCard>
{
    public void Configure(EntityTypeBuilder<GiftCard> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Status).IsRequired().HasConversion<int>();
        builder.Property(g => g.Amount).HasPrecision(18, 2);
        builder.HasOne(g => g.User).WithMany().HasForeignKey(g => g.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(g => g.Payment).WithMany().HasForeignKey(g => g.PaymentId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
    }
}
