using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Payouts.Commands.ApprovePayout
{
    public class ApprovePayoutResult
    {
        public Guid PayoutId { get; }
        public string Status { get; }

        public ApprovePayoutResult(Guid payoutId, string status)
        {
            PayoutId = payoutId;
            Status = status;
        }
    }
}