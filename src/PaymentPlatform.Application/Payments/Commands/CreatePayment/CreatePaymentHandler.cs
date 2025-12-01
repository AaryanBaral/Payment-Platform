
using PaymentPlatform.Application.Common;
using PaymentPlatform.Application.Messaging;
using PaymentPlatform.Application.Presistence;
using PaymentPlatform.Domain.Payment;

namespace PaymentPlatform.Application.Payments.Commands.CreatePayment
{
    public class CreatePaymentHandler : ICommandHandler<CreatePaymentCommand, Result<CreatePaymentResult>>
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePaymentHandler(
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

        public async Task<Result<CreatePaymentResult>> HandleAsync(
            CreatePaymentCommand command,
            CancellationToken cancellationToken = default)
        {
            // validate tenant
            var tenant = await _tenantRepository.GetByIdAsync(command.TenantId, cancellationToken);
            if (tenant is null)
            {
                return Result<CreatePaymentResult>.Failure("Tenant not found or inactive.");
            }
            // validate merchant
            var merchant = await _merchantRepository.GetByIdAsync(command.MerchantId, cancellationToken);
            if (merchant is null || merchant.TenantId != command.TenantId)
            {
                return Result<CreatePaymentResult>.Failure("Merchant not found or inactive.");
            }
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
                return Result<CreatePaymentResult>.Failure(ex.Message);
            }
            // 4. Persist
            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            var result = new CreatePaymentResult(
                payment.Id,
                payment.TenantId,
                payment.MerchantId,
                payment.Amount.Amount,
                payment.Amount.Currency
            );

            return Result<CreatePaymentResult>.Success(result);

        }
    }
}