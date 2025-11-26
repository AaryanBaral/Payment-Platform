using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentPlatform.Domain.Common;

namespace PaymentPlatform.Domain.Tenant
{
    public class TenantUser : Entity
    {
        public Guid TenantId { get; private set; }
        public string Email { get; private set; } = default!;
        public string DisplayName { get; private set; } = default!;
        public TenantUserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTimeOffset JoinedAtUtc { get; private set; }

        // For EF Core
        private TenantUser() { }

        private TenantUser(
            Guid tenantId,
            string email,
            string displayName,
            TenantUserRole role,
            DateTimeOffset joinedAtUtc)
            : base()
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name is required.", nameof(displayName));

            TenantId = tenantId;
            Email = email.Trim().ToLowerInvariant();
            DisplayName = displayName.Trim();
            Role = role;
            JoinedAtUtc = joinedAtUtc;
            IsActive = true;
        }

        // Factory: create a tenant user
        public static TenantUser Create(
            Guid tenantId,
            string email,
            string displayName,
            TenantUserRole role,
            DateTimeOffset joinedAtUtc)
        {
            return new TenantUser(tenantId, email, displayName, role, joinedAtUtc);
        }

        public void ChangeRole(TenantUserRole newRole)
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot change role of an inactive user.");

            Role = newRole;
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new InvalidOperationException("User is already inactive.");

            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("User is already active.");

            IsActive = true;
        }

        public void UpdateProfile(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name is required.", nameof(displayName));

            DisplayName = displayName.Trim();
        }
    }
}