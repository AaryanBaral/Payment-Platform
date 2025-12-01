using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;

namespace PaymentPlatform.Application.Commands.RecordIncomingPayment
{
    public class RecordIncomingPaymentCommand : ICommand<Result<RecordIncomingPaymentResult>>
    {
        public Guid TenantId { get; }
        public Guid MerchantId { get; }
        public decimal Amount { get; }
        public string Currency { get; }
        public string ExternalPaymentId { get; }

        public RecordIncomingPaymentCommand(
            Guid tenantId,
            Guid merchantId,
            decimal amount,
            string currency,
            string externalPaymentId)
        {
            TenantId = tenantId;
            MerchantId = merchantId;
            Amount = amount;
            Currency = currency;
            ExternalPaymentId = externalPaymentId;
        }
    }
}