using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentPlatform.Domain.Payment;

namespace PaymentPlatform.Infrastructure.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.TenantId)
                .IsRequired();

            builder.Property(p => p.MerchantId)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.ExternalPaymentId)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.CreatedAtUtc)
                .IsRequired();

            builder.Property(p => p.CompletedAtUtc);

            // Map Money as an owned type on Payment.Amount
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
                .HasDatabaseName("IX_Payments_Tenant_Merchant_Status");
        }
    }
}