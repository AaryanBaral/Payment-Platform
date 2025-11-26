using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentPlatform.Domain.Tenant;

namespace PaymentPlatform.Infrastructure.Persistence.Configurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            // Table name
            builder.ToTable("Tenants");

            // Primary key
            builder.HasKey(t => t.Id);

            // Properties
            builder.Property(t => t.Id)
                .ValueGeneratedNever(); // we use Guid from the domain, not DB-generated

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.DefaultCurrency)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(t => t.IsActive)
                .IsRequired();

            builder.Property(t => t.CreatedAtUtc)
                .IsRequired();

            builder.Property(t => t.DeactivatedAtUtc);

            // Index on Name (optional, but useful for lookup)
            builder.HasIndex(t => t.Name)
                .HasDatabaseName("IX_Tenants_Name");
        }
    }
}