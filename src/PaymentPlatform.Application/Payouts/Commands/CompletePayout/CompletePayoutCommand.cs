using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;

namespace PaymentPlatform.Application.Payouts.Commands.CompletePayout
{
    public class CompletePayoutCommand : ICommand<Result<CompletePayoutResult>>
    {
        public Guid TenantId { get; }
        public Guid PayoutId { get; }
        public Guid CompletedByUserId { get; }
        public DateTimeOffset CompletedAtUtc { get; }
        public string? Reference { get; }

        public CompletePayoutCommand(
            Guid tenantId,
            Guid payoutId,
            Guid completedByUserId,
            DateTimeOffset completedAtUtc,
            string? reference)
        {
            TenantId = tenantId;
            PayoutId = payoutId;
            CompletedByUserId = completedByUserId;
            CompletedAtUtc = completedAtUtc;
            Reference = reference;
        }
    }
}