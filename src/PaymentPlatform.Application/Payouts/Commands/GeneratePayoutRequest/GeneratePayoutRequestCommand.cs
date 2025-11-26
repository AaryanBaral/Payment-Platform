
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;

namespace PaymentPlatform.Application.Payouts.Commands.GeneratePayoutRequest
{
    public class GeneratePayoutRequestCommand : ICommand<Result<GeneratePayoutRequestResult>>
    {
        public Guid TenantId { get; }
        public Guid MerchantId { get; }
        public decimal RequestedAmount { get; }
        public string Currency { get; }
        public Guid RequestedByUserId { get; }

        public GeneratePayoutRequestCommand(
            Guid tenantId,
            Guid merchantId,
            decimal requestedAmount,
            string currency,
            Guid requestedByUserId)
        {
            TenantId = tenantId;
            MerchantId = merchantId;
            RequestedAmount = requestedAmount;
            Currency = currency;
            RequestedByUserId = requestedByUserId;
        }

    }
}