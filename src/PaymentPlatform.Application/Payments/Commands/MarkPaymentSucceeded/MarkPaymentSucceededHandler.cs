
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Ledger;

namespace PaymentPlatform.Application.Payments.Commands.CreatePayment
{
    public class MarkPaymentSucceededHandler
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILedgerEntryRepository _ledgerEntryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MarkPaymentSucceededHandler(
            ITenantRepository tenantRepository,
            IMerchantRepository merchantRepository,
            IPaymentRepository paymentRepository,
            ILedgerEntryRepository ledgerEntryRepository,
            IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _merchantRepository = merchantRepository;
            _paymentRepository = paymentRepository;
            _ledgerEntryRepository = ledgerEntryRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<MarkPaymentSucceededResult>> HandleAsync(
    MarkPaymentSucceededCommand command,
    CancellationToken cancellationToken = default)
        {
            // 1. Validate tenant
            var tenant = await _tenantRepository.GetByIdAsync(command.TenantId, cancellationToken);
            if (tenant is null)
            {
                return Result<MarkPaymentSucceededResult>.Failure("Tenant not found or inactive.");
            }
            // 2. Load payment
            var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken);
            if (payment is null || payment.TenantId != command.TenantId)
            {
                return Result<MarkPaymentSucceededResult>.Failure("Payment not found for this tenant.");
            }
            // 3. Ensure it's still pending
            if (payment.Status != Domain.Payment.PaymentStatus.Pending)
            {
                return Result<MarkPaymentSucceededResult>.Failure("Only pending payments can be marked as succeeded.");
            }

            // 4. Mark payment as succeeded
            payment.MarkSucceeded(command.CompletedAtUtc);

            // 5. Load merchant to get revenue share
            var merchant = await _merchantRepository.GetByIdAsync(payment.MerchantId, cancellationToken);
            if (merchant is null || merchant.TenantId != command.TenantId)
            {
                return Result<MarkPaymentSucceededResult>.Failure("Merchant not found for this tenant.");
            }

            // 6. Calculate revenue split
            var (merchantAmount, platformAmount) =
                payment.CalculateRevenueSplit(merchant.RevenueShare);

            // 7. Create ledger entries
            var merchantCredit = LedgerEntry.CreateMerchantCredit(
                tenantId: payment.TenantId,
                merchantId: payment.MerchantId,
                amount: merchantAmount,
                sourceType: LedgerEntrySourceType.Payment,
                sourceId: payment.Id,
                description: $"Payment {payment.ExternalPaymentId} - merchant share",
                occurredAtUtc: command.CompletedAtUtc);

            var platformCredit = LedgerEntry.CreatePlatformCredit(
                tenantId: payment.TenantId,
                amount: platformAmount,
                sourceType: LedgerEntrySourceType.Payment,
                sourceId: payment.Id,
                description: $"Payment {payment.ExternalPaymentId} - platform share",
                occurredAtUtc: command.CompletedAtUtc);

            await _ledgerEntryRepository.AddAsync(merchantCredit, cancellationToken);
            await _ledgerEntryRepository.AddAsync(platformCredit, cancellationToken);

            // 8. Commit
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new MarkPaymentSucceededResult(
                payment.Id,
                payment.TenantId,
                payment.MerchantId,
                merchantAmount.Amount,
                platformAmount.Amount,
                merchantAmount.Currency);

            return Result<MarkPaymentSucceededResult>.Success(result);
        }
    }
}