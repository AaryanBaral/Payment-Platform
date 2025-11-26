using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Ledger;
using PaymentPlatform.Domain.Merchant;
using PaymentPlatform.Domain.Payment;
using PaymentPlatform.Domain.Payout;
using PaymentPlatform.Domain.Tenant;

namespace PaymentPlatform.Infrastructure.Persistence
{
public class AppDbContext : DbContext, IUnitOfWork
{
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantUser> TenantUsers => Set<TenantUser>();
    public DbSet<Merchant> Merchants => Set<Merchant>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<LedgerEntry> LedgerEntries => Set<LedgerEntry>();
    public DbSet<Payout> Payouts => Set<Payout>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // This is the IUnitOfWork implementation
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically apply all IEntityTypeConfiguration<T> in this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
}