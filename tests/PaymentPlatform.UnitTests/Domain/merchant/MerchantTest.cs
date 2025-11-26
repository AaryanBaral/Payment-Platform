

using PaymentPlatform.Domain.Merchants;

namespace PaymentPlatform.UnitTests.Domain.merchant
{
    public class MerchantTest
    {

        [Fact]
        public void Create_WithValidData_ShouldCreateMerchant()
        {
            //static values
            var tenantId = Guid.NewGuid();
            var name = "Test Shop";
            var email = "shop@example.com";
            decimal share = 80m;

            var merchant = Merchant.Create(tenantId, name, email, share);
            Assert.Equal(tenantId, merchant.TenantId);
            Assert.Equal(name, merchant.Name);
            Assert.Equal(email, merchant.Email);
            Assert.Equal(share, merchant.RevenueShare.Value);
            Assert.True(merchant.IsActive);
            Assert.Equal(0.8m, merchant.RevenueShare.AsFraction());
            Assert.NotEqual(Guid.Empty, merchant.TenantId);
        }
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidName_ShouldThrow(string? name)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var email = "shop@example.com";

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => Merchant.Create(tenantId, name!, email, 80m));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidEmail_ShouldThrow(string? email)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var name = "Test Shop";

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => Merchant.Create(tenantId, name, email!, 80m));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(100)]
        [InlineData(150)]
        public void Create_WithInvalidRevenueShare_ShouldThrow(decimal share)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var name = "Test Shop";
            var email = "shop@example.com";

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () => Merchant.Create(tenantId, name, email, share));
        }

        [Fact]
        public void UpdateDetails_WithValidData_ShouldUpdateNameAndEmail()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchant = Merchant.Create(tenantId, "Old Name", "old@example.com", 80m);

            // Act
            merchant.UpdateDetails("New Name", "new@example.com");

            // Assert
            Assert.Equal("New Name", merchant.Name);
            Assert.Equal("new@example.com", merchant.Email);
        }

        [Fact]
        public void UpdateRevenueShare_WithValidPercentage_ShouldUpdateShare()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchant = Merchant.Create(tenantId, "Shop", "shop@example.com", 80m);

            // Act
            merchant.UpdateRevenueShare(50m);

            // Assert
            Assert.Equal(50m, merchant.RevenueShare.Value);
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveFalse()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchant = Merchant.Create(tenantId, "Shop", "shop@example.com", 80m);

            // Act
            merchant.Deactivate();

            // Assert
            Assert.False(merchant.IsActive);
        }

        [Fact]
        public void Activate_ShouldSetIsActiveTrue()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var merchant = Merchant.Create(tenantId, "Shop", "shop@example.com", 80m);
            merchant.Deactivate();

            // Act
            merchant.Activate();

            // Assert
            Assert.True(merchant.IsActive);
        }
    }
}