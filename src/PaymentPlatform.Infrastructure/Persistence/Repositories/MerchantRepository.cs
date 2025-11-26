using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Merchant;

namespace PaymentPlatform.Infrastructure.Persistence.Repositories
{
    public class MerchantRepository : IMerchantRepository
    {
        private readonly AppDbContext _dbContext;

        public MerchantRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<Merchant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbContext.Merchants
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }
    }
}