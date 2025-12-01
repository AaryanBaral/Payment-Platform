using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Api.Models.Payouts
{
    public class RejectPayoutRequestDto
    {
        public Guid TenantId { get; set; }
        public Guid RejectedByUserId { get; set; }
        public DateTimeOffset RejectedAtUtc { get; set; }
        public string? Notes { get; set; }
    }
}