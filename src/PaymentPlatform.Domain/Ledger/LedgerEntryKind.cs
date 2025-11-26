namespace PaymentPlatform.Domain.Ledger
{
    public enum LedgerEntryKind
    {
        MerchantCredit = 1,   // Merchant's balance increases (e.g., from a payment)
        MerchantDebit = 2,    // Merchant's balance decreases (e.g., payout)
        PlatformCredit = 3,   // Platform's revenue increases
        PlatformDebit = 4     // Platform's revenue decreases (e.g., refund, adjustment)
    }

}