
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;
using PaymentPlatform.Application.Payouts.Services;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Common;
using PaymentPlatform.Domain.Payout;

namespace PaymentPlatform.Application.Payouts.Commands.GeneratePayoutRequest
{
    public class GeneratePayoutRequestHandler : ICommandHandler<GeneratePayoutRequestCommand, Result<GeneratePayoutRequestResult>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IPayoutRepository _payoutRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMerchantBalanceService _merchantBalanceService;

        public GeneratePayoutRequestHandler(ITenantRepository tenantRepository, IMerchantRepository merchantRepository, IPayoutRepository payoutRepository, IUnitOfWork unitOfWork, IMerchantBalanceService merchantBalanceService)
        {
            _merchantRepository = merchantRepository;
            _payoutRepository = payoutRepository;
            _tenantRepository = tenantRepository;
            _merchantBalanceService = merchantBalanceService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<GeneratePayoutRequestResult>> HandleAsync(GeneratePayoutRequestCommand command, CancellationToken cancellationToken = default)
        {
            // 1. Basic input validation
            if (command.RequestedAmount <= 0)
            {
                return Result<GeneratePayoutRequestResult>.Failure("Requested amount must be positive.");
            }

            if (string.IsNullOrWhiteSpace(command.Currency))
            {
                return Result<GeneratePayoutRequestResult>.Failure("Currency is required.");
            }

            // 2. Ensure tenant exists and is active
            var tenant = await _tenantRepository.GetByIdAsync(command.TenantId, cancellationToken);
            if (tenant is null || !tenant.IsActive)
            {
                return Result<GeneratePayoutRequestResult>.Failure("Tenant not found or inactive.");
            }

            // 3. Ensure merchant exists, belongs to tenant, and is active
            var merchant = await _merchantRepository.GetByIdAsync(command.MerchantId, cancellationToken);
            if (merchant is null || merchant.TenantId != command.TenantId)
            {
                return Result<GeneratePayoutRequestResult>.Failure("Merchant not found for this tenant.");
            }

            if (!merchant.IsActive)
            {
                return Result<GeneratePayoutRequestResult>.Failure("Merchant is inactive.");
            }

            // 4. Get merchant balance
            Money merchantBalance =
                await _merchantBalanceService.GetBalanceAsync(command.TenantId, command.MerchantId, cancellationToken);

            if (merchantBalance.Amount <= 0m)
            {
                return Result<GeneratePayoutRequestResult>.Failure("Merchant has no available balance for payout.");
            }

            // Optional: ensure currency matches balance currency (if you support multiple currencies)
            if (!string.Equals(merchantBalance.Currency, command.Currency, StringComparison.OrdinalIgnoreCase))
            {
                return Result<GeneratePayoutRequestResult>.Failure("Requested currency does not match merchant balance currency.");
            }

            // 5. Ensure requested amount does not exceed available balance
            if (merchantBalance.Amount < command.RequestedAmount)
            {
                return Result<GeneratePayoutRequestResult>.Failure("Requested amount exceeds available balance.");
            }

            // 6. Create payout in domain
            Payout payout;
            try
            {
                payout = Payout.Request(
                    command.TenantId,
                    command.MerchantId,
                    command.RequestedAmount,
                    command.Currency,
                    command.RequestedByUserId,
                    DateTimeOffset.UtcNow);
            }
            catch (ArgumentException ex)
            {
                // Domain validation errors (e.g., invalid amount/currency)
                return Result<GeneratePayoutRequestResult>.Failure(ex.Message);
            }

            // 7. Persist payout
            await _payoutRepository.AddAsync(payout, cancellationToken);

            // 8. Save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 9. Map to result DTO
            var result = new GeneratePayoutRequestResult(
                payout.Id,
                payout.TenantId,
                payout.MerchantId,
                payout.Amount.Amount,
                payout.Amount.Currency,
                payout.Status.ToString());

            return Result<GeneratePayoutRequestResult>.Success(result);
        }

    }
}
