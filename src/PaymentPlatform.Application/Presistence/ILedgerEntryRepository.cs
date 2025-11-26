using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentPlatform.Domain.Ledger;

namespace PaymentPlatform.Application.Presistence
{
    public interface ILedgerEntryRepository
    {
         Task AddAsync(LedgerEntry entry, CancellationToken cancellationToken = default);
    }
}