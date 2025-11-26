
using PaymentPlatform.Domain.Common;

namespace PaymentPlatform.Domain.Merchants
{
    // A merchant that belongs to a tenant and has a revenue share.
    public class Merchant : Entity
    {
        public Guid TenantId { get; private set; }
        public string Name { get; private set; } = default!;
        public string Email { get; private set; } = default!;
        public Percentage RevenueShare { get; private set; }= default!;
        public bool IsActive { get; private set; }


        // For EF Core or serializers
        private Merchant() { }
        private Merchant(Guid tenantId, string name, string email, Percentage revenueShare)
        : base()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Merchant name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Merchant email is required.", nameof(email));

            TenantId = tenantId;
            Name = name.Trim();
            Email = email.Trim();
            RevenueShare = revenueShare;
            IsActive = true;
        }

        // Factory method to create a merchant.
        public static Merchant Create(Guid tenantId, string name, string email, decimal revenueSharePercentage)
        {
            var percentage = Percentage.From(revenueSharePercentage);
            return new Merchant(tenantId, name, email, percentage);
        }

        public void UpdateDetails(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Merchant name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Merchant email is required.", nameof(email));

            Name = name.Trim();
            Email = email.Trim();
        }

        public void UpdateRevenueShare(decimal revenueSharePercentage)
        {
            RevenueShare = Percentage.From(revenueSharePercentage);
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Activate()
        {
            IsActive = true;
        }
    }
}