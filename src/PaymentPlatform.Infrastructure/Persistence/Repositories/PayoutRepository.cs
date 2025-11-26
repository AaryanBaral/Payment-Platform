using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Payout;

namespace PaymentPlatform.Infrastructure.Persistence.Repositories
{
    public class PayoutRepository:IPayoutRepository
    {
        private readonly AppDbContext _dbContext;
        public PayoutRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(Payout payout, CancellationToken cancellationToken = default)
        {
            await _dbContext.Payouts.AddAsync(payout, cancellationToken);
        }
        public async Task<Payout?> GetByIdAsync(Guid payoutId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Payouts.FirstOrDefaultAsync(p=>p.Id == payoutId, cancellationToken);
        }
    }
}