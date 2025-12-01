using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PaymentPlatform.Application.Payments.Commands.CreatePayment
{
    public class MarkPaymentSucceededResult
    {
        public Guid PaymentId { get; }
        public Guid TenantId { get; }
        public Guid MerchantId { get; }
        public decimal MerchantAmount { get; }
        public decimal PlatformAmount { get; }
        public string Currency { get; }

        public MarkPaymentSucceededResult(
            Guid paymentId,
            Guid tenantId,
            Guid merchantId,
            decimal merchantAmount,
            decimal platformAmount,
            string currency)
        {
            PaymentId = paymentId;
            TenantId = tenantId;
            MerchantId = merchantId;
            MerchantAmount = merchantAmount;
            PlatformAmount = platformAmount;
            Currency = currency;
        }
    }
}