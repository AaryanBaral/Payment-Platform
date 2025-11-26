using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentPlatform.Domain.Common;

namespace PaymentPlatform.Domain.Tenant
{
    public class Tenant : Entity
    {
        public string Name { get; private set; } = default!;
        public string DefaultCurrency { get; private set; } = default!; 
        public bool IsActive { get; private set; }
        public DateTimeOffset CreatedAtUtc { get; private set; }
        public DateTimeOffset? DeactivatedAtUtc { get; private set; }

        // For EF Core
        private Tenant() { }

        private Tenant(string name, string defaultCurrency, DateTimeOffset createdAtUtc)
            : base()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tenant name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(defaultCurrency))
                throw new ArgumentException("Default currency is required.", nameof(defaultCurrency));

            Name = name.Trim();
            DefaultCurrency = defaultCurrency.Trim().ToUpperInvariant();
            CreatedAtUtc = createdAtUtc;
            IsActive = true;
            DeactivatedAtUtc = null;
        }

        // Factory method to create a new tenant
        public static Tenant Create(string name, string defaultCurrency, DateTimeOffset createdAtUtc)
        {
            return new Tenant(name, defaultCurrency, createdAtUtc);
        }

        public void Rename(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("Tenant name is required.", nameof(newName));

            Name = newName.Trim();
        }

        public void Deactivate(DateTimeOffset deactivatedAtUtc)
        {
            if (!IsActive)
                throw new InvalidOperationException("Tenant is already inactive.");

            IsActive = false;
            DeactivatedAtUtc = deactivatedAtUtc;
        }

        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("Tenant is already active.");

            IsActive = true;
            DeactivatedAtUtc = null;
        }
    }
}