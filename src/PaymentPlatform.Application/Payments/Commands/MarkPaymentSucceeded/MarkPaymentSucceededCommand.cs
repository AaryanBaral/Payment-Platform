using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Payments.Commands.CreatePayment
{
    public class MarkPaymentSucceededCommand
    {
                public Guid TenantId { get; }
        public Guid PaymentId { get; }
        public DateTimeOffset CompletedAtUtc { get; }

        public MarkPaymentSucceededCommand(
            Guid tenantId,
            Guid paymentId,
            DateTimeOffset completedAtUtc)
        {
            TenantId = tenantId;
            PaymentId = paymentId;
            CompletedAtUtc = completedAtUtc;
        }
    }
}