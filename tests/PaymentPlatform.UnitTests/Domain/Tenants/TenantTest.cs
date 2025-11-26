

using PaymentPlatform.Domain.Tenant;

namespace PaymentPlatform.UnitTests.Domain.Tenants
{
    public class TenantTest
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateActiveTenant()
        {
            // Arrange
            var name = "Test Tenant";
            var currency = "npr";
            var createdAt = DateTimeOffset.UtcNow;

            // Act
            var tenant = Tenant.Create(name, currency, createdAt);

            // Assert
            Assert.Equal(name, tenant.Name);
            Assert.Equal("NPR", tenant.DefaultCurrency); // should be uppercased
            Assert.Equal(createdAt, tenant.CreatedAtUtc);
            Assert.True(tenant.IsActive);
            Assert.Null(tenant.DeactivatedAtUtc);
            Assert.NotEqual(Guid.Empty, tenant.Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidName_ShouldThrow(string? name)
        {
            // Arrange
            var currency = "NPR";
            var createdAt = DateTimeOffset.UtcNow;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Tenant.Create(name!, currency, createdAt));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidCurrency_ShouldThrow(string? currency)
        {
            // Arrange
            var name = "Test Tenant";
            var createdAt = DateTimeOffset.UtcNow;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                Tenant.Create(name, currency!, createdAt));
        }

        [Fact]
        public void Rename_WithValidName_ShouldUpdateName()
        {
            // Arrange
            var tenant = Tenant.Create("Old Name", "NPR", DateTimeOffset.UtcNow);

            // Act
            tenant.Rename("New Name");

            // Assert
            Assert.Equal("New Name", tenant.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Rename_WithInvalidName_ShouldThrow(string? newName)
        {
            // Arrange
            var tenant = Tenant.Create("Old Name", "NPR", DateTimeOffset.UtcNow);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => tenant.Rename(newName!));
        }

        [Fact]
        public void Deactivate_FromActive_ShouldSetInactiveAndSetDeactivatedAt()
        {
            // Arrange
            var tenant = Tenant.Create("Test Tenant", "NPR", DateTimeOffset.UtcNow);
            var deactivatedAt = DateTimeOffset.UtcNow;

            // Act
            tenant.Deactivate(deactivatedAt);

            // Assert
            Assert.False(tenant.IsActive);
            Assert.Equal(deactivatedAt, tenant.DeactivatedAtUtc);
        }

        [Fact]
        public void Deactivate_WhenAlreadyInactive_ShouldThrow()
        {
            // Arrange
            var tenant = Tenant.Create("Test Tenant", "NPR", DateTimeOffset.UtcNow);
            tenant.Deactivate(DateTimeOffset.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                tenant.Deactivate(DateTimeOffset.UtcNow));
        }

        [Fact]
        public void Activate_FromInactive_ShouldSetActiveAndClearDeactivatedAt()
        {
            // Arrange
            var tenant = Tenant.Create("Test Tenant", "NPR", DateTimeOffset.UtcNow);
            tenant.Deactivate(DateTimeOffset.UtcNow);

            // Act
            tenant.Activate();

            // Assert
            Assert.True(tenant.IsActive);
            Assert.Null(tenant.DeactivatedAtUtc);
        }

        [Fact]
        public void Activate_WhenAlreadyActive_ShouldThrow()
        {
            // Arrange
            var tenant = Tenant.Create("Test Tenant", "NPR", DateTimeOffset.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                tenant.Activate());
        }
    }
}