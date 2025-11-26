using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Tenant;

namespace PaymentPlatform.Infrastructure.Persistence.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly AppDbContext _dbContext;

        public TenantRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return _dbContext.Tenants
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
        public async Task<IReadOnlyList<Tenant>> GetAll(CancellationToken cancellationToken)
        {
            return await _dbContext.Tenants.AsNoTracking().ToListAsync(cancellationToken);
        }
    }
}