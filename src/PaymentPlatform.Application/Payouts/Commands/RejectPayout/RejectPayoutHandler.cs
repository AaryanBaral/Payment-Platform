using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Payout;

namespace PaymentPlatform.Application.Payouts.Commands.RejectPayout
{
    public class RejectPayoutHandler : ICommandHandler<RejectPayoutCommand, Result<RejectPayoutResult>>
    {
         
        private readonly ITenantRepository _tenantRepository;
        private readonly IPayoutRepository _payoutRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RejectPayoutHandler(
            ITenantRepository tenantRepository,
            IPayoutRepository payoutRepository,
            IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _payoutRepository = payoutRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<RejectPayoutResult>> HandleAsync(
            RejectPayoutCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate tenant
            var tenant = await _tenantRepository.GetByIdAsync(command.TenantId, cancellationToken);
            if (tenant is null)
            {
                return Result<RejectPayoutResult>.Failure("Tenant not found or inactive.");
            }

            // 2. Load payout
            var payout = await _payoutRepository.GetByIdAsync(command.PayoutId, cancellationToken);
            if (payout is null || payout.TenantId != command.TenantId)
            {
                return Result<RejectPayoutResult>.Failure("Payout not found for this tenant.");
            }

            // 3. Only requested payouts can be rejected
            if (payout.Status != PayoutStatus.Requested)
            {
                return Result<RejectPayoutResult>.Failure("Only requested payouts can be rejected.");
            }
            
            // 4. Domain operation
            payout.Reject(
                command.RejectedByUserId,
                command.RejectedAtUtc,
                command.Notes);

            // 5. Persist
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 6. Build result
            var result = new RejectPayoutResult(
                payout.Id,
                payout.TenantId,
                payout.MerchantId,
                payout.Amount.Amount,
                payout.Amount.Currency,
                payout.Status.ToString(),
                payout.Notes);

            return Result<RejectPayoutResult>.Success(result);
        }
    }
}
