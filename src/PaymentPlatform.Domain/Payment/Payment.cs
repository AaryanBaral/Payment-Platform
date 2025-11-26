

using PaymentPlatform.Domain.Common;
using PaymentPlatform.Domain.Merchant;

namespace PaymentPlatform.Domain.Payment
{
    public class Payment : Entity
    {
        public Guid TenantId { get; private set; }
        public Guid MerchantId { get; private set; }
        public Money Amount { get; private set; } = default!;
        public PaymentStatus Status { get; private set; }
        public string ExternalPaymentId { get; private set; } = default!;
        public DateTimeOffset CreatedAtUtc { get; private set; }
        public DateTimeOffset? CompletedAtUtc { get; private set; }
        private Payment() { }
        private Payment(Guid tenantId,
        Guid merchantId,
        Money amount,
        string externalPaymentId) : base()
        {

            // validations and setter code for this class
            if (amount is null)
                throw new ArgumentNullException(nameof(amount));

            if (amount.Amount <= 0)
                throw new ArgumentException("Payment amount must be positive.", nameof(amount));

            if (string.IsNullOrWhiteSpace(externalPaymentId))
                throw new ArgumentException("External payment ID is required.", nameof(externalPaymentId));

            TenantId = tenantId;
            MerchantId = merchantId;
            Amount = amount;
            ExternalPaymentId = externalPaymentId.Trim();

            Status = PaymentStatus.Pending;
            CreatedAtUtc = DateTimeOffset.UtcNow;
            CompletedAtUtc = null;
        }

        // public function that is to be called my other class
        // this methods internally calls the constructor and retuern it.
        public static Payment CreatePending(Guid tenantId,
        Guid merchantId,
        decimal amount,
        string currency,
        string externalPaymentId)
        {
            var money = Money.From(amount, currency);
            return new Payment(tenantId, merchantId, money, externalPaymentId);

        }

        // mark the payment success after buisness processing.
        public void MarkSucceeded(DateTimeOffset completedAtUtc)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be marked as succeeded.");

            Status = PaymentStatus.Succeeded;
            CompletedAtUtc = completedAtUtc;
        }

        // Mark the payment as failed (after gateway callback)
        public void MarkFailed(DateTimeOffset completedAtUtc)
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be marked as failed.");

            Status = PaymentStatus.Failed;
            CompletedAtUtc = completedAtUtc;
        }

        public (Money merchantamount,Money platformAmount ) CalculateRevenueSplit(Percentage merchantShare)
        {
                    if (merchantShare is null)
            throw new ArgumentNullException(nameof(merchantShare));
                    if (Status != PaymentStatus.Succeeded)
            throw new InvalidOperationException("Revenue can only be split for succeeded payments.");

            // multiply the amount by the fraction of the merchant share
            var merchantAmount = Amount.Multiply(merchantShare.AsFraction());

            var platformAmount = Amount.Subtract(merchantAmount);
            return (merchantAmount, platformAmount);
        }


    }
}