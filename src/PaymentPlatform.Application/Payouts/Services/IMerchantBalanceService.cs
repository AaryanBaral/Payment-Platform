using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentPlatform.Domain.Common;

namespace PaymentPlatform.Application.Payouts.Services
{
    public interface IMerchantBalanceService
    {
        Task<Money> GetBalanceAsync(Guid tenantId, Guid merchantId, CancellationToken ct);
    }
}