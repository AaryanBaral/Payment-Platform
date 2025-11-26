using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Commands.RecordIncomingPayment
{
    public class RecordIncomingPaymentResult
    {
        public Guid PaymentId { get; }
        public Guid TenantId { get; }
        public Guid MerchantId { get; }
        public decimal Amount { get; }
        public string Currency { get; }
        public string Status { get; }

        public RecordIncomingPaymentResult(
            Guid paymentId,
            Guid tenantId,
            Guid merchantId,
            decimal amount,
            string currency,
            string status)
        {
            PaymentId = paymentId;
            TenantId = tenantId;
            MerchantId = merchantId;
            Amount = amount;
            Currency = currency;
            Status = status;
        }
    }
}