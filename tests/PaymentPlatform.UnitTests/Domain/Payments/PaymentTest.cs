
using PaymentPlatform.Domain.Common;
using PaymentPlatform.Domain.Payment;


namespace PaymentPlatform.UnitTests.Domain.Payments
{
    public class PaymentTest
    {
        [Fact]
        public void CreatePending_WithValidData_ShouldCreatePaymentWithPendingStatus()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            decimal amount = 1000m;
            string currency = "NPR";
            string externalPaymentId = "ext-123";

            // Act
            var payment = Payment.CreatePending(
                tenantId,
                merchantId,
                amount,
                currency,
                externalPaymentId);

            // Assert
            Assert.Equal(tenantId, payment.TenantId);
            Assert.Equal(merchantId, payment.MerchantId);
            Assert.Equal(amount, payment.Amount.Amount);
            Assert.Equal("NPR", payment.Amount.Currency);
            Assert.Equal(PaymentStatus.Pending, payment.Status);
            Assert.Equal(externalPaymentId, payment.ExternalPaymentId);
            Assert.True(payment.CreatedAtUtc <= DateTimeOffset.UtcNow);
            Assert.Null(payment.CompletedAtUtc);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public void CreatePending_WithNonPositiveAmount_ShouldThrow(decimal amount)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Payment.CreatePending(tenantId, merchantId, amount, "NPR", "ext-123"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreatePending_WithInvalidExternalPaymentId_ShouldThrow(string? externalId)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Payment.CreatePending(tenantId, merchantId, 1000m, "NPR", externalId!));
        }

        [Fact]
        public void MarkSucceeded_FromPending_ShouldUpdateStatusAndCompletedAt()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var payment = Payment.CreatePending(tenantId, merchantId, 1000m, "NPR", "ext-123");
            var completedAt = DateTimeOffset.UtcNow;

            // Act
            payment.MarkSucceeded(completedAt);

            // Assert
            Assert.Equal(PaymentStatus.Succeeded, payment.Status);
            Assert.Equal(completedAt, payment.CompletedAtUtc);
        }

        [Fact]
        public void MarkFailed_FromPending_ShouldUpdateStatusAndCompletedAt()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var payment = Payment.CreatePending(tenantId, merchantId, 1000m, "NPR", "ext-123");
            var completedAt = DateTimeOffset.UtcNow;

            // Act
            payment.MarkFailed(completedAt);

            // Assert
            Assert.Equal(PaymentStatus.Failed, payment.Status);
            Assert.Equal(completedAt, payment.CompletedAtUtc);
        }

        [Fact]
        public void MarkSucceeded_WhenNotPending_ShouldThrow()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var payment = Payment.CreatePending(tenantId, merchantId, 1000m, "NPR", "ext-123");
            payment.MarkSucceeded(DateTimeOffset.UtcNow); // now succeeded

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                payment.MarkSucceeded(DateTimeOffset.UtcNow));
        }

        [Fact]
        public void MarkFailed_WhenNotPending_ShouldThrow()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchantId = Guid.NewGuid();
            var payment = Payment.CreatePending(tenantId, merchantId, 1000m, "NPR", "ext-123");
            payment.MarkFailed(DateTimeOffset.UtcNow); // now failed

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                payment.MarkFailed(DateTimeOffset.UtcNow));
        }

        [Fact]
    public void CalculateRevenueSplit_WithValidPercentage_ShouldReturnCorrectShares()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var merchantId = Guid.NewGuid();
        var payment = Payment.CreatePending(tenantId, merchantId, 1000m, "NPR", "ext-123");
        payment.MarkSucceeded(DateTimeOffset.UtcNow);

        var merchantShare = Percentage.From(80m); // 80%

        // Act
        var (merchantAmount, platformAmount) = payment.CalculateRevenueSplit(merchantShare);

        // Assert
        Assert.Equal(800m, merchantAmount.Amount);
        Assert.Equal("NPR", merchantAmount.Currency);

        Assert.Equal(200m, platformAmount.Amount);
        Assert.Equal("NPR", platformAmount.Currency);
    }

    [Fact]
    public void CalculateRevenueSplit_WhenPaymentNotSucceeded_ShouldThrow()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var merchantId = Guid.NewGuid();
        var payment = Payment.CreatePending(tenantId, merchantId, 1000m, "NPR", "ext-123");
        var merchantShare = Percentage.From(80m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(
            () => payment.CalculateRevenueSplit(merchantShare));
    }
    }
}
