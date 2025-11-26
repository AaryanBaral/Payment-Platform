using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Payment;

namespace PaymentPlatform.Application.Commands.RecordIncomingPayment
{
    public class RecordIncomingPaymentHandler : ICommandHandler<RecordIncomingPaymentCommand, Result<RecordIncomingPaymentResult>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RecordIncomingPaymentHandler(
        ITenantRepository tenantRepository,
        IMerchantRepository merchantRepository,
        IPaymentRepository paymentRepository,
        IUnitOfWork unitOfWork)
        {
            _tenantRepository = tenantRepository;
            _merchantRepository = merchantRepository;
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result<RecordIncomingPaymentResult>> HandleAsync(
    RecordIncomingPaymentCommand command,
    CancellationToken cancellationToken = default)
        {
            // 1. try to get and verify if tenant exists
            var tenant = await _tenantRepository.GetByIdAsync(command.TenantId, cancellationToken);
            if (tenant is null || !tenant.IsActive)
            {
                return Result<RecordIncomingPaymentResult>.Failure("Tenant not found or inactive.");
            }

            // 2. Ensure merchant exists and belongs to the tenant
            var merchant = await _merchantRepository.GetByIdAsync(command.MerchantId, cancellationToken);
            if (merchant is null || merchant.TenantId != command.TenantId)
            {
                return Result<RecordIncomingPaymentResult>.Failure("Merchant not found for this tenant.");
            }
            // 3. Validate basic input (we could put more validation here or in a separate validator)
            if (command.Amount <= 0)
            {
                return Result<RecordIncomingPaymentResult>.Failure("Amount must be positive.");
            }

            if (string.IsNullOrWhiteSpace(command.Currency))
            {
                return Result<RecordIncomingPaymentResult>.Failure("Currency is required.");
            }

            if (string.IsNullOrWhiteSpace(command.ExternalPaymentId))
            {
                return Result<RecordIncomingPaymentResult>.Failure("External payment id is required.");
            }
            // 4. Create a Pending payment in the domain
            Payment payment;
            try
            {
                payment = Payment.CreatePending(
                    command.TenantId,
                    command.MerchantId,
                    command.Amount,
                    command.Currency,
                    command.ExternalPaymentId);
            }
            catch (ArgumentException ex)
            {
                // Wrap domain validation errors into a failure result
                return Result<RecordIncomingPaymentResult>.Failure(ex.Message);
            }

            // 5. Save payment
            await _paymentRepository.AddAsync(payment, cancellationToken);

            // 6. Commit transaction / save changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Build result DTO
            var result = new RecordIncomingPaymentResult(
                payment.Id,
                payment.TenantId,
                payment.MerchantId,
                payment.Amount.Amount,
                payment.Amount.Currency,
                payment.Status.ToString());

            return Result<RecordIncomingPaymentResult>.Success(result);

        }
    }
}