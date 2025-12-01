using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Api.Models.Payment
{
    public class MarkPaymentSucceededRequestDto
    {
        public Guid TenantId { get; set; }
        public DateTimeOffset CompletedAtUtc { get; set; }
    }
}