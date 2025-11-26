

namespace PaymentPlatform.Domain.Payout
{
    public enum PayoutStatus
    {
        Requested = 1,   // Created but not approved yet
        Approved = 2,    // Approved by finance/admin, but not yet sent
        Completed = 3,   // Money has been sent and confirmed
        Rejected = 4
    }
}