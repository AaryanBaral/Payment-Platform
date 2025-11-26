using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Payout;

namespace PaymentPlatform.Application.Payouts.Commands.ApprovePayout
{
    public class ApprovePayoutHandler
        : ICommandHandler<ApprovePayoutCommand, Result<ApprovePayoutResult>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IPayoutRepository _payoutRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ApprovePayoutHandler(
            ITenantRepository tenantRepository,
            IPayoutRepository payoutRepository,
            IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _payoutRepository = payoutRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ApprovePayoutResult>> HandleAsync(
            ApprovePayoutCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Check tenant exists and is active
            var tenant = await _tenantRepository.GetByIdAsync(command.TenantId, cancellationToken);
            if (tenant is null || !tenant.IsActive)
            {
                return Result<ApprovePayoutResult>.Failure("Tenant not found or inactive.");
            }

            // 2. Load payout
            var payout = await _payoutRepository.GetByIdAsync(command.PayoutId, cancellationToken);
            if (payout is null || payout.TenantId != command.TenantId)
            {
                return Result<ApprovePayoutResult>.Failure("Payout not found for this tenant.");
            }

            // 3. Only requested payouts can be approved
            if (payout.Status != PayoutStatus.Requested)
            {
                return Result<ApprovePayoutResult>.Failure("Only requested payouts can be approved.");
            }

            // 4. Domain-level state change
            try
            {
                payout.Approve(command.ApprovedByUserId, DateTimeOffset.UtcNow);
            }
            catch (InvalidOperationException ex)
            {
                return Result<ApprovePayoutResult>.Failure(ex.Message);
            }

            // 5. Save
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. Map to result
            var result = new ApprovePayoutResult(
                payout.Id,
                payout.Status.ToString());

            return Result<ApprovePayoutResult>.Success(result);
        }
    }
}