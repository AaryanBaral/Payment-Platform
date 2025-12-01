using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Payouts.Commands.RejectPayout
{
    public class RejectPayoutResult
    {
                public Guid PayoutId { get; }
        public Guid TenantId { get; }
        public Guid MerchantId { get; }
        public decimal Amount { get; }
        public string Currency { get; }
        public string Status { get; }
        public string? Notes { get; }

        public RejectPayoutResult(
            Guid payoutId,
            Guid tenantId,
            Guid merchantId,
            decimal amount,
            string currency,
            string status,
            string? notes)
        {
            PayoutId = payoutId;
            TenantId = tenantId;
            MerchantId = merchantId;
            Amount = amount;
            Currency = currency;
            Status = status;
            Notes = notes;
        }
    }
}