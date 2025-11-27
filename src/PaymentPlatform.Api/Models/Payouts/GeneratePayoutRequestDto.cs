using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Api.Models.Payouts
{
    public class GeneratePayoutRequestDto
    {
        public Guid TenantId { get; set; }
        public Guid MerchantId { get; set; }
        public decimal RequestedAmount { get; set; }
        public string Currency { get; set; } = default!;
        public Guid RequestedByUserId { get; set; }
    }
}