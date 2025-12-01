using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Ledger;
using PaymentPlatform.Domain.Payment;

namespace PaymentPlatform.Application.Commands.ConfirmPaymentSucceeded
{
    public class ConfirmPaymentSucceededHandler : ICommandHandler<ConfirmPaymentSucceededCommand, Result>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly ILedgerEntryRepository _ledgerEntryRepository;
        private readonly IUnitOfWork _unitOfWork;


        public ConfirmPaymentSucceededHandler(
            IPaymentRepository paymentRepository,
            IMerchantRepository merchantRepository,
            ILedgerEntryRepository ledgerEntryRepository,
            IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _merchantRepository = merchantRepository;
            _ledgerEntryRepository = ledgerEntryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> HandleAsync(
    ConfirmPaymentSucceededCommand command,
    CancellationToken cancellationToken = default)
        {
            var payment = await _paymentRepository.GetByIdAsync(command.PaymentId, cancellationToken);
            if (payment is null)
                return Result.Failure("Payment not found.");

            if (payment.Status != PaymentStatus.Pending)
            {
                return Result.Failure("Only pending payments can be marked as succeeded.");
            }
            try
            {
                payment.MarkSucceeded(command.CompletedAtUtc);
            }
            catch (InvalidOperationException ex)
            {
                // Should be rare, but we still turn it into a clean failure
                return Result.Failure(ex.Message);
            }

            // Load the merchant to get revenue share
            var merchant = await _merchantRepository.GetByIdAsync(payment.MerchantId, cancellationToken);
            if (merchant is null || !merchant.IsActive)
            {
                // This is a serious data problem, but we'll surface a failure for now.
                return Result.Failure("Merchant not found or inactive for this payment.");
            }
            // calculating the share split
            var (merchantShare, platformShare) = payment.CalculateRevenueSplit(merchant.RevenueShare);


            // Create ledger entries

            // Merchant credit (they earned money from this payment)
            var merchantCreditEntry = LedgerEntry.CreateMerchantCredit(
                tenantId: payment.TenantId,
                merchantId: payment.MerchantId,
                amount: merchantShare,
                sourceType: LedgerEntrySourceType.Payment,
                sourceId: payment.Id,
                description: $"Merchant share from payment {payment.Id}",
                occurredAtUtc: command.CompletedAtUtc);

            //  Platform credit (platform revenue from this payment)
            var platformCreditEntry = LedgerEntry.CreatePlatformCredit(
                tenantId: payment.TenantId,
                amount: platformShare,
                sourceType: LedgerEntrySourceType.Payment,
                sourceId: payment.Id,
                description: $"Platform share from payment {payment.Id}",
                occurredAtUtc: command.CompletedAtUtc);

            //  Persist ledger entries
            await _ledgerEntryRepository.AddAsync(merchantCreditEntry, cancellationToken);
            await _ledgerEntryRepository.AddAsync(platformCreditEntry, cancellationToken);

            // 8 Save all changes (payment + ledger entries) in a transaction
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
    }
}