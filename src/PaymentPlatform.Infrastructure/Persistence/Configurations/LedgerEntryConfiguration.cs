
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentPlatform.Domain.Ledger;

namespace PaymentPlatform.Infrastructure.Persistence.Configurations
{
    public class LedgerEntryConfiguration : IEntityTypeConfiguration<LedgerEntry>
    {
        public void Configure(EntityTypeBuilder<LedgerEntry> builder)
        {
            builder.ToTable("LedgerEntries");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .ValueGeneratedNever();

            builder.Property(l => l.TenantId)
                .IsRequired();

            builder.Property(l => l.MerchantId);

            builder.Property(l => l.Kind)
                .IsRequired();

            builder.Property(l => l.SourceType)
                .IsRequired();

            builder.Property(l => l.SourceId)
                .IsRequired();

            builder.Property(l => l.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(l => l.OccurredAtUtc)
                .IsRequired();

            // Money as owned type again
            builder.OwnsOne(l => l.Amount, money =>
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

            // Indexes for reporting
            builder.HasIndex(l => new { l.TenantId, l.MerchantId, l.OccurredAtUtc })
                .HasDatabaseName("IX_LedgerEntries_Tenant_Merchant_Date");
        }
    }
}