using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentPlatform.Domain.Payout;

namespace PaymentPlatform.Infrastructure.Persistence.Configurations
{
    public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
    {
        public void Configure(EntityTypeBuilder<Payout> builder)
        {
            builder.ToTable("Payouts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.TenantId)
                .IsRequired();

            builder.Property(p => p.MerchantId)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.RequestedAtUtc)
                .IsRequired();

            builder.Property(p => p.RequestedByUserId)
                .IsRequired();

            builder.Property(p => p.ApprovedAtUtc);
            builder.Property(p => p.ApprovedByUserId);
            builder.Property(p => p.CompletedAtUtc);
            builder.Property(p => p.CompletedByUserId);
            builder.Property(p => p.RejectedAtUtc);
            builder.Property(p => p.RejectedByUserId);

            builder.Property(p => p.Reference)
                .HasMaxLength(200);

            builder.Property(p => p.Notes)
                .HasMaxLength(1000);

            // Money as owned type again
            builder.OwnsOne(p => p.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.HasIndex(p => new { p.TenantId, p.MerchantId, p.Status })
                .HasDatabaseName("IX_Payouts_Tenant_Merchant_Status");
        }
    }
}