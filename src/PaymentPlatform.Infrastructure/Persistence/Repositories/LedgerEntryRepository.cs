using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Ledger;

namespace PaymentPlatform.Infrastructure.Persistence.Repositories
{
    public class LedgerEntryRepository : ILedgerEntryRepository
    {
        private readonly AppDbContext _dbContext;

        public LedgerEntryRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(LedgerEntry ledgerEntry, CancellationToken cancellationToken = default)
        {
            await _dbContext.LedgerEntries.AddAsync(ledgerEntry, cancellationToken);
        }
    }
}