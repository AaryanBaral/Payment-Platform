using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Presistence
{
    public interface IUnitOfWork
    {
            Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}