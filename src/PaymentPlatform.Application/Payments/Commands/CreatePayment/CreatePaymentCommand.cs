
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;

namespace PaymentPlatform.Application.Payments.Commands.CreatePayment
{
    public class CreatePaymentCommand : ICommand<Result<CreatePaymentResult>>
    {
        public Guid TenantId { get; }
        public Guid MerchantId { get; }
        public decimal Amount { get; }
        public string Currency { get; }
        public string ExternalPaymentId { get; }

        public CreatePaymentCommand(
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