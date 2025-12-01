using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Domain.Common;
using PaymentPlatform.Domain.Ledger;
using PaymentPlatform.Domain.Merchant;
using PaymentPlatform.Domain.Tenant;

namespace PaymentPlatform.Infrastructure.Persistence
{
    public class DbSeeders
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            // Apply migrations if needed (for real DB; safe to call multiple times)
            await db.Database.MigrateAsync();

            if (await db.Tenants.AnyAsync())
            {
                // Already seeded
                Console.WriteLine("Already Seeded.");
                return;
            }

            // 1. Seed Tenant
            var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var tenant = Tenant.Create(
                name: "Test Tenant",
                defaultCurrency: "NPR", 
                DateTimeOffset.UtcNow);

            tenant.GetType().GetProperty("Id")!
                .SetValue(tenant, tenantId);
            await db.Tenants.AddAsync(tenant);

            Console.WriteLine("Tenants Seeded.");

            // 2. Seed Merchant
            var merchantId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            Console.WriteLine(merchantId);
            var merchant = Merchant.Create(
                tenantId,
                name: "Test Merchant",
                email: "merchant@example.com",
                revenueSharePercentage: 80m);

            // We need to force the generated Id to something predictable for testing
            // (optional; you can also use merchant.Id as-is)
            merchant.GetType().GetProperty("Id")!
                .SetValue(merchant, merchantId);

            await db.Merchants.AddAsync(merchant);
            Console.WriteLine("Merchants Seeded.");

            // 3. Seed a Ledger credit so merchant has balance
            var ledgerEntry = LedgerEntry.CreateMerchantCredit(
                tenantId: tenantId,
                merchantId: merchantId,
                amount: Money.From(2000m, "NPR"),
                sourceType: LedgerEntrySourceType.Payment,
                sourceId: Guid.Parse("33333333-3333-3333-3333-333333333333"),
                description: "Seed credit for testing payouts",
                occurredAtUtc: DateTimeOffset.UtcNow);

            await db.LedgerEntries.AddAsync(ledgerEntry);
            Console.WriteLine("Ledger Seeded.");

            await db.SaveChangesAsync();
        }
    }
}