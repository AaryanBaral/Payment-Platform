using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentPlatform.Domain.Common;
using PaymentPlatform.Domain.Merchant;

namespace PaymentPlatform.Infrastructure.Persistence.Configurations
{
    public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            builder.ToTable("Merchants");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedNever();

            builder.Property(m => m.TenantId)
                .IsRequired();

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.IsActive)
                .IsRequired();

            // Map Percentage value object to a decimal column
            builder.Property(m => m.RevenueShare)
                .HasConversion(
                    percentage => percentage.Value,          // to decimal for DB
                    value => Percentage.From(value))         // to Percentage for domain
                .HasColumnName("RevenueShare")
                .HasColumnType("decimal(5,2)")              // e.g. 80.00 = 80%
                .IsRequired();

            // Useful unique index for (Tenant, Email)
            builder.HasIndex(m => new { m.TenantId, m.Email })
                .HasDatabaseName("IX_Merchants_Tenant_Email")
                .IsUnique();
        }
    }
}