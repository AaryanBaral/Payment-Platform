
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;

namespace PaymentPlatform.Application.Payouts.Commands.ApprovePayout
{
public class ApprovePayoutCommand : ICommand<Result<ApprovePayoutResult>>
{
    public Guid TenantId { get; }
    public Guid PayoutId { get; }
    public Guid ApprovedByUserId { get; }

    public ApprovePayoutCommand(Guid tenantId, Guid payoutId, Guid approvedByUserId)
    {
        TenantId = tenantId;
        PayoutId = payoutId;
        ApprovedByUserId = approvedByUserId;
    }
}
}