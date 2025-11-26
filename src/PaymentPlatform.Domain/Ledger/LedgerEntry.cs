
using PaymentPlatform.Domain.Common;

namespace PaymentPlatform.Domain.Ledger
{
    public class LedgerEntry : Entity
    {
        public Guid TenantId { get; private set; }

        // Null for platform-level entries (e.g., platform revenue),
        // set for merchant entries (e.g., merchant earnings, merchant payouts).
        public Guid? MerchantId { get; private set; }

        public Money Amount { get; private set; } = default!;
        public LedgerEntryKind Kind { get; private set; }
        public LedgerEntrySourceType SourceType { get; private set; }
        public Guid SourceId { get; private set; }          // e.g., Payment.Id, Payout.Id
        public string Description { get; private set; } = default!;
        public DateTimeOffset OccurredAtUtc { get; private set; }

        public LedgerEntry()
        {


        }
        public LedgerEntry(Guid tenantId,
        Guid? merchantId,
        Money amount,
        LedgerEntryKind kind,
        LedgerEntrySourceType sourceType,
        Guid sourceId,
        string description,
        DateTimeOffset occurredAtUtc) : base()
        {

            if (amount is null)
                throw new ArgumentNullException(nameof(amount));

            if (amount.Amount <= 0)
                throw new ArgumentException("Ledger amount must be positive.", nameof(amount));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.", nameof(description));

            TenantId = tenantId;
            MerchantId = merchantId;
            Amount = amount;
            Kind = kind;
            SourceType = sourceType;
            SourceId = sourceId;
            Description = description.Trim();
            OccurredAtUtc = occurredAtUtc;
        }
        public static LedgerEntry CreateMerchantCredit(
    Guid tenantId,
    Guid merchantId,
    Money amount,
    LedgerEntrySourceType sourceType,
    Guid sourceId,
    string description,
    DateTimeOffset occurredAtUtc)
        {
            return new LedgerEntry(
                tenantId,
                merchantId,
                amount,
                LedgerEntryKind.MerchantCredit,
                sourceType,
                sourceId,
                description,
                occurredAtUtc);
        }
        public static LedgerEntry CreateMerchantDebit(
        Guid tenantId,
        Guid merchantId,
        Money amount,
        LedgerEntrySourceType sourceType,
        Guid sourceId,
        string description,
        DateTimeOffset occurredAtUtc)
        {
            return new LedgerEntry(
                tenantId,
                merchantId,
                amount,
                LedgerEntryKind.MerchantDebit,
                sourceType,
                sourceId,
                description,
                occurredAtUtc);
        }

        public static LedgerEntry CreatePlatformCredit(
        Guid tenantId,
        Money amount,
        LedgerEntrySourceType sourceType,
        Guid sourceId,
        string description,
        DateTimeOffset occurredAtUtc)
        {
            return new LedgerEntry(
                tenantId,
                merchantId: null,
                amount,
                LedgerEntryKind.PlatformCredit,
                sourceType,
                sourceId,
                description,
                occurredAtUtc);
        }
        public static LedgerEntry CreatePlatformDebit(
        Guid tenantId,
        Money amount,
        LedgerEntrySourceType sourceType,
        Guid sourceId,
        string description,
        DateTimeOffset occurredAtUtc)
        {
            return new LedgerEntry(
                tenantId,
                merchantId: null,
                amount,
                LedgerEntryKind.PlatformDebit,
                sourceType,
                sourceId,
                description,
                occurredAtUtc);
        }
    }
}