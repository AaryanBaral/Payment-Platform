using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Ledger;
using PaymentPlatform.Domain.Payout;

namespace PaymentPlatform.Application.Payouts.Commands.CompletePayout
{
    public class CompletePayoutHandler
        : ICommandHandler<CompletePayoutCommand, Result<CompletePayoutResult>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IPayoutRepository _payoutRepository;
        private readonly ILedgerEntryRepository _ledgerEntryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompletePayoutHandler(
            ITenantRepository tenantRepository,
            IPayoutRepository payoutRepository,
            ILedgerEntryRepository ledgerEntryRepository,
            IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _payoutRepository = payoutRepository;
            _ledgerEntryRepository = ledgerEntryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CompletePayoutResult>> HandleAsync(
            CompletePayoutCommand command,
            CancellationToken cancellationToken = default)
        {
            // 1. Check tenant
            var tenant = await _tenantRepository.GetByIdAsync(command.TenantId, cancellationToken);
            if (tenant is null || !tenant.IsActive)
            {
                return Result<CompletePayoutResult>.Failure("Tenant not found or inactive.");
            }

            // 2. Load payout
            var payout = await _payoutRepository.GetByIdAsync(command.PayoutId, cancellationToken);
            if (payout is null || payout.TenantId != command.TenantId)
            {
                return Result<CompletePayoutResult>.Failure("Payout not found for this tenant.");
            }

            // 3. Only approved payouts can be completed
            if (payout.Status != PayoutStatus.Approved)
            {
                return Result<CompletePayoutResult>.Failure("Only approved payouts can be completed.");
            }

            // 4. Domain change: mark as completed
            try
            {
                payout.MarkCompleted(
                    command.CompletedByUserId,
                    command.CompletedAtUtc,
                    command.Reference);
            }
            catch (InvalidOperationException ex)
            {
                return Result<CompletePayoutResult>.Failure(ex.Message);
            }

            // 5. Create ledger entry: merchant debit (their balance goes down)
            var merchantDebitEntry = LedgerEntry.CreateMerchantDebit(
                tenantId: payout.TenantId,
                merchantId: payout.MerchantId,
                amount: payout.Amount,
                sourceType: LedgerEntrySourceType.Payout,
                sourceId: payout.Id,
                description: $"Payout {payout.Id} completed",
                occurredAtUtc: command.CompletedAtUtc);

            await _ledgerEntryRepository.AddAsync(merchantDebitEntry, cancellationToken);

            // 6. Save all changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Map to result
            var result = new CompletePayoutResult(
                payout.Id,
                payout.Status.ToString());

            return Result<CompletePayoutResult>.Success(result);
        }
    }
}