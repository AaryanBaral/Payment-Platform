using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentPlatform.Domain.Common;

namespace PaymentPlatform.Domain.Payout
{
    public class Payout:Entity
    {
        public Guid TenantId { get; private set; }
        public Guid MerchantId { get; private set; }
        public Money Amount { get; private set; } = default!;
        public PayoutStatus Status { get; private set; }

        public DateTimeOffset RequestedAtUtc { get; private set; }
        public Guid RequestedByUserId { get; private set; }

        public DateTimeOffset? ApprovedAtUtc { get; private set; }
        public Guid? ApprovedByUserId { get; private set; }

        public DateTimeOffset? CompletedAtUtc { get; private set; }
        public Guid? CompletedByUserId { get; private set; }

        public DateTimeOffset? RejectedAtUtc { get; private set; }
        public Guid? RejectedByUserId { get; private set; }

        public string? Reference { get; private set; }
        public string? Notes { get; private set; }

        private Payout() { }

        private Payout(
            Guid tenantId,
            Guid merchantId,
            Money amount,
            Guid requestedByUserId,
            DateTimeOffset requestedAtUtc,
            string? reference,
            string? notes)
            : base()
        {
            if (amount is null)
                throw new ArgumentNullException(nameof(amount));

            if (amount.Amount <= 0)
                throw new ArgumentException("Payout amount must be positive.", nameof(amount));

            TenantId = tenantId;
            MerchantId = merchantId;
            Amount = amount;

            RequestedByUserId = requestedByUserId;
            RequestedAtUtc = requestedAtUtc;

            Status = PayoutStatus.Requested;

            Reference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim();
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }


        public static Payout Request(
    Guid tenantId,
    Guid merchantId,
    decimal amount,
    string currency,
    Guid requestedByUserId,
    DateTimeOffset requestedAtUtc,
    string? reference = null,
    string? notes = null)
        {
            var money = Money.From(amount, currency);
            return new Payout(
                tenantId,
                merchantId,
                money,
                requestedByUserId,
                requestedAtUtc,
                reference,
                notes);
        }

        public void Approve(Guid approvedByUserId, DateTimeOffset approvedAtUtc)
        {
            if (Status != PayoutStatus.Requested)
                throw new InvalidOperationException("Only requested payouts can be approved.");

            Status = PayoutStatus.Approved;
            ApprovedByUserId = approvedByUserId;
            ApprovedAtUtc = approvedAtUtc;
        }

        public void MarkCompleted(Guid completedByUserId, DateTimeOffset completedAtUtc, string? reference = null)
        {
            if (Status != PayoutStatus.Approved)
                throw new InvalidOperationException("Only approved payouts can be completed.");

            Status = PayoutStatus.Completed;
            CompletedByUserId = completedByUserId;
            CompletedAtUtc = completedAtUtc;

            if (!string.IsNullOrWhiteSpace(reference))
            {
                Reference = reference.Trim();
            }
        }

        public void Reject(Guid rejectedByUserId, DateTimeOffset rejectedAtUtc, string? notes = null)
        {
            if (Status != PayoutStatus.Requested)
                throw new InvalidOperationException("Only requested payouts can be rejected.");

            Status = PayoutStatus.Rejected;
            RejectedByUserId = rejectedByUserId;
            RejectedAtUtc = rejectedAtUtc;

            if (!string.IsNullOrWhiteSpace(notes))
            {
                Notes = notes.Trim();
            }
        }
    }
}