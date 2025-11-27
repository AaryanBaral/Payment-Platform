using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Application.Payouts.Services;
using PaymentPlatform.Domain.Common;
using PaymentPlatform.Domain.Ledger;
using PaymentPlatform.Infrastructure.Persistence;

namespace PaymentPlatform.Infrastructure.Payouts
{
    public class MerchantBalanceService : IMerchantBalanceService
    {

        private readonly AppDbContext _dbContext;

        public MerchantBalanceService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Money> GetBalanceAsync(
            Guid tenantId,
            Guid merchantId,
            CancellationToken cancellationToken = default)
        {
            // 1. Load tenant to get its default currency (assumes Tenant has DefaultCurrency)
            var tenant = await _dbContext.Tenants
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == tenantId, cancellationToken);

            if (tenant is null)
            {
                throw new InvalidOperationException(
                    $"Tenant '{tenantId}' not found when calculating merchant balance.");
            }

            var defaultCurrency = tenant.DefaultCurrency; // e.g. "NPR", "USD"

            // 2. Load all ledger entries for this tenant + merchant
            //    We only need Kind, Amount, Currency.
            var entries = await _dbContext.LedgerEntries
                .Where(e => e.TenantId == tenantId && e.MerchantId == merchantId)
                .Select(e => new
                {
                    e.Kind,
                    Amount = e.Amount.Amount,
                    Currency = e.Amount.Currency
                })
                .ToListAsync(cancellationToken);

            // 3. If there are no ledger entries yet → balance is zero in tenant's default currency
            if (entries.Count == 0)
            {
                return Money.Zero(defaultCurrency);
            }

            // 4. Ensure all entries use a single currency (simple assumption for now)
            var currency = entries[0].Currency;
            foreach (var entry in entries)
            {
                if (!string.Equals(entry.Currency, currency, StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Merchant ledger contains multiple currencies. This is not supported yet.");
                }
            }

            // 5. Compute balance = credits - debits
            decimal balanceAmount = 0m;

            foreach (var entry in entries)
            {
                switch (entry.Kind)
                {
                    case LedgerEntryKind.MerchantCredit:
                        balanceAmount += entry.Amount;
                        break;

                    case LedgerEntryKind.MerchantDebit:
                        balanceAmount -= entry.Amount;
                        break;

                    // Platform credits/debits don't affect the merchant's balance
                    case LedgerEntryKind.PlatformCredit:
                    case LedgerEntryKind.PlatformDebit:
                    default:
                        break;
                }
            }

            // 6. Guard against negative due to bugs or inconsistencies.
            //    Your Money.From() does not allow negative amounts.
            if (balanceAmount < 0)
            {
                // You could also throw here if you want strictness instead of clamping.
                balanceAmount = 0m;
            }

            return Money.From(balanceAmount, currency);
        }
    }
}