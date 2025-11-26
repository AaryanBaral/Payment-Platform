using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentPlatform.Domain.Payout;

namespace PaymentPlatform.Application.Presistence
{
    public interface IPayoutRepository
    {
        Task AddAsync(Payout payout, CancellationToken cancellationToken = default);
        Task<Payout?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}