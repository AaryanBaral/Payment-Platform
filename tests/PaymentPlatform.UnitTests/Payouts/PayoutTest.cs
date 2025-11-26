using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentPlatform.Domain.Payout;

namespace PaymentPlatform.UnitTests.Payouts
{
    public class PayoutTest
    {
        [Fact]
        public void Request_WithValidData_ShouldCreateRequestedPayout()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            decimal amount = 2400m;
            string currency = "NPR";
            var requestedByUserId = Guid.NewGuid();
            var requestedAt = DateTimeOffset.UtcNow;
            string? reference = "batch-001";
            string? notes = "Weekly payout";

            // Act
            var payout = Payout.Request(
                tenantId,
                merchantId,
                amount,
                currency,
                requestedByUserId,
                requestedAt,
                reference,
                notes);

            // Assert
            Assert.Equal(tenantId, payout.TenantId);
            Assert.Equal(merchantId, payout.MerchantId);
            Assert.Equal(amount, payout.Amount.Amount);
            Assert.Equal("NPR", payout.Amount.Currency);
            Assert.Equal(PayoutStatus.Requested, payout.Status);
            Assert.Equal(requestedByUserId, payout.RequestedByUserId);
            Assert.Equal(requestedAt, payout.RequestedAtUtc);
            Assert.Equal(reference, payout.Reference);
            Assert.Equal(notes, payout.Notes);
            Assert.NotEqual(Guid.Empty, payout.Id);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void Request_WithNonPositiveAmount_ShouldThrow(decimal amount)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var requestedByUserId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Payout.Request(
                    tenantId,
                    merchantId,
                    amount,
                    "NPR",
                    requestedByUserId,
                    DateTimeOffset.UtcNow));
        }

        [Fact]
        public void Approve_FromRequested_ShouldSetStatusAndAuditFields()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var requestedByUserId = Guid.NewGuid();
            var payout = Payout.Request(
                tenantId,
                merchantId,
                2400m,
                "NPR",
                requestedByUserId,
                DateTimeOffset.UtcNow);

            var approvedByUserId = Guid.NewGuid();
            var approvedAt = DateTimeOffset.UtcNow;

            // Act
            payout.Approve(approvedByUserId, approvedAt);

            // Assert
            Assert.Equal(PayoutStatus.Approved, payout.Status);
            Assert.Equal(approvedByUserId, payout.ApprovedByUserId);
            Assert.Equal(approvedAt, payout.ApprovedAtUtc);
        }

        [Fact]
        public void Approve_WhenNotRequested_ShouldThrow()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var requestedByUserId = Guid.NewGuid();
            var payout = Payout.Request(
                tenantId,
                merchantId,
                2400m,
                "NPR",
                requestedByUserId,
                DateTimeOffset.UtcNow);

            payout.Approve(Guid.NewGuid(), DateTimeOffset.UtcNow); // now Approved

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                payout.Approve(Guid.NewGuid(), DateTimeOffset.UtcNow));
        }

        [Fact]
        public void MarkCompleted_FromApproved_ShouldSetStatusAndAuditFields()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var requestedByUserId = Guid.NewGuid();
            var payout = Payout.Request(
                tenantId,
                merchantId,
                2400m,
                "NPR",
                requestedByUserId,
                DateTimeOffset.UtcNow);

            payout.Approve(Guid.NewGuid(), DateTimeOffset.UtcNow);

            var completedByUserId = Guid.NewGuid();
            var completedAt = DateTimeOffset.UtcNow;
            var reference = "bank-tx-999";

            // Act
            payout.MarkCompleted(completedByUserId, completedAt, reference);

            // Assert
            Assert.Equal(PayoutStatus.Completed, payout.Status);
            Assert.Equal(completedByUserId, payout.CompletedByUserId);
            Assert.Equal(completedAt, payout.CompletedAtUtc);
            Assert.Equal(reference, payout.Reference);
        }

        [Fact]
        public void MarkCompleted_WhenNotApproved_ShouldThrow()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var requestedByUserId = Guid.NewGuid();
            var payout = Payout.Request(
                tenantId,
                merchantId,
                2400m,
                "NPR",
                requestedByUserId,
                DateTimeOffset.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                payout.MarkCompleted(Guid.NewGuid(), DateTimeOffset.UtcNow));
        }

        [Fact]
        public void Reject_FromRequested_ShouldSetStatusAndAuditFields()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var requestedByUserId = Guid.NewGuid();
            var payout = Payout.Request(
                tenantId,
                merchantId,
                2400m,
                "NPR",
                requestedByUserId,
                DateTimeOffset.UtcNow);

            var rejectedByUserId = Guid.NewGuid();
            var rejectedAt = DateTimeOffset.UtcNow;
            string? notes = "Insufficient balance";

            // Act
            payout.Reject(rejectedByUserId, rejectedAt, notes);

            // Assert
            Assert.Equal(PayoutStatus.Rejected, payout.Status);
            Assert.Equal(rejectedByUserId, payout.RejectedByUserId);
            Assert.Equal(rejectedAt, payout.RejectedAtUtc);
            Assert.Equal(notes, payout.Notes);
        }

        [Fact]
        public void Reject_WhenNotRequested_ShouldThrow()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var requestedByUserId = Guid.NewGuid();
            var payout = Payout.Request(
                tenantId,
                merchantId,
                2400m,
                "NPR",
                requestedByUserId,
                DateTimeOffset.UtcNow);

            payout.Approve(Guid.NewGuid(), DateTimeOffset.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                payout.Reject(Guid.NewGuid(), DateTimeOffset.UtcNow));
        }
    }
}