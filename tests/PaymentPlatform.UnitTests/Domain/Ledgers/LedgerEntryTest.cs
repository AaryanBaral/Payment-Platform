using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PaymentPlatform.Domain.Common;
using PaymentPlatform.Domain.Ledger;

namespace PaymentPlatform.UnitTests.Domain.Ledgers
{
    public class LedgerEntryTest
    {
        [Fact]
        public void CreateMerchantCredit_WithValidData_ShouldCreateEntry()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var amount = Money.From(1000m, "NPR");
            var sourceType = LedgerEntrySourceType.Payment;
            var sourceId = Guid.NewGuid();
            var description = "Merchant share from payment";
            var occurredAt = DateTimeOffset.UtcNow;

            // Act
            var entry = LedgerEntry.CreateMerchantCredit(
                tenantId,
                merchantId,
                amount,
                sourceType,
                sourceId,
                description,
                occurredAt);

            // Assert
            Assert.Equal(tenantId, entry.TenantId);
            Assert.Equal(merchantId, entry.MerchantId);
            Assert.Equal(amount, entry.Amount);
            Assert.Equal(LedgerEntryKind.MerchantCredit, entry.Kind);
            Assert.Equal(sourceType, entry.SourceType);
            Assert.Equal(sourceId, entry.SourceId);
            Assert.Equal(description, entry.Description);
            Assert.Equal(occurredAt, entry.OccurredAtUtc);
            Assert.NotEqual(Guid.Empty, entry.Id);
        }

        [Fact]
        public void CreatePlatformCredit_WithValidData_ShouldHaveNullMerchantId()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var amount = Money.From(200m, "NPR");
            var sourceType = LedgerEntrySourceType.Payment;
            var sourceId = Guid.NewGuid();
            var description = "Platform share from payment";
            var occurredAt = DateTimeOffset.UtcNow;

            // Act
            var entry = LedgerEntry.CreatePlatformCredit(
                tenantId,
                amount,
                sourceType,
                sourceId,
                description,
                occurredAt);

            // Assert
            Assert.Equal(tenantId, entry.TenantId);
            Assert.Null(entry.MerchantId);
            Assert.Equal(amount, entry.Amount);
            Assert.Equal(LedgerEntryKind.PlatformCredit, entry.Kind);
            Assert.Equal(sourceType, entry.SourceType);
            Assert.Equal(sourceId, entry.SourceId);
            Assert.Equal(description, entry.Description);
            Assert.Equal(occurredAt, entry.OccurredAtUtc);
        }

        [Fact]
        public void Constructor_WithNonPositiveAmount_ShouldThrow()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var badAmount = Money.From(0m, "NPR"); // Money itself allows 0; Ledger adds stricter rule.

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                LedgerEntry.CreateMerchantCredit(
                    tenantId,
                    merchantId,
                    badAmount,
                    LedgerEntrySourceType.Payment,
                    Guid.NewGuid(),
                    "Invalid amount",
                    DateTimeOffset.UtcNow));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidDescription_ShouldThrow(string? description)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var amount = Money.From(100m, "NPR");

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                LedgerEntry.CreateMerchantCredit(
                    tenantId,
                    merchantId,
                    amount,
                    LedgerEntrySourceType.Payment,
                    Guid.NewGuid(),
                    description!,
                    DateTimeOffset.UtcNow));
        }
        

    }
}