using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Payments.Commands.CreatePayment
{
    public class CreatePaymentResult
    {
        public Guid PaymentId { get; }
        public Guid TenantId { get; }
        public Guid MerchantId { get; }
        public decimal Amount { get; }
        public string Currency { get; }

        public CreatePaymentResult(
            Guid paymentId,
            Guid tenantId,
            Guid merchantId,
            decimal amount,
            string currency)
        {
            PaymentId = paymentId;
            TenantId = tenantId;
            MerchantId = merchantId;
            Amount = amount;
            Currency = currency;
        }
    }
}