

using PaymentPlatform.Domain.Tenant;

namespace PaymentPlatform.UnitTests.Domain.Tenants
{
    public class TenantUserTest
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateActiveUser()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var email = "User@Example.com";
            var displayName = "Test User";
            var role = TenantUserRole.Admin;
            var joinedAt = DateTimeOffset.UtcNow;

            // Act
            var user = TenantUser.Create(
                tenantId,
                email,
                displayName,
                role,
                joinedAt);

            // Assert
            Assert.Equal(tenantId, user.TenantId);
            Assert.Equal(email.ToLowerInvariant(), user.Email);
            Assert.Equal(displayName, user.DisplayName);
            Assert.Equal(role, user.Role);
            Assert.Equal(joinedAt, user.JoinedAtUtc);
            Assert.True(user.IsActive);
            Assert.NotEqual(Guid.Empty, user.Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidEmail_ShouldThrow(string? email)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var displayName = "Test User";
            var role = TenantUserRole.Admin;
            var joinedAt = DateTimeOffset.UtcNow;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                TenantUser.Create(
                    tenantId,
                    email!,
                    displayName,
                    role,
                    joinedAt));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithInvalidDisplayName_ShouldThrow(string? displayName)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var email = "user@example.com";
            var role = TenantUserRole.Admin;
            var joinedAt = DateTimeOffset.UtcNow;

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                TenantUser.Create(
                    tenantId,
                    email,
                    displayName!,
                    role,
                    joinedAt));
        }

        [Fact]
        public void ChangeRole_WhenActive_ShouldUpdateRole()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var user = TenantUser.Create(
                tenantId,
                "user@example.com",
                "Test User",
                TenantUserRole.Viewer,
                DateTimeOffset.UtcNow);

            // Act
            user.ChangeRole(TenantUserRole.Finance);

            // Assert
            Assert.Equal(TenantUserRole.Finance, user.Role);
        }

        [Fact]
        public void ChangeRole_WhenInactive_ShouldThrow()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var user = TenantUser.Create(
                tenantId,
                "user@example.com",
                "Test User",
                TenantUserRole.Viewer,
                DateTimeOffset.UtcNow);

            user.Deactivate();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                user.ChangeRole(TenantUserRole.Admin));
        }

        [Fact]
        public void Deactivate_FromActive_ShouldSetIsActiveFalse()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var user = TenantUser.Create(
                tenantId,
                "user@example.com",
                "Test User",
                TenantUserRole.Viewer,
                DateTimeOffset.UtcNow);

            // Act
            user.Deactivate();

            // Assert
            Assert.False(user.IsActive);
        }

        [Fact]
        public void Deactivate_WhenAlreadyInactive_ShouldThrow()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var user = TenantUser.Create(
                tenantId,
                "user@example.com",
                "Test User",
                TenantUserRole.Viewer,
                DateTimeOffset.UtcNow);

            user.Deactivate();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                user.Deactivate());
        }

        [Fact]
        public void Activate_FromInactive_ShouldSetIsActiveTrue()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var user = TenantUser.Create(
                tenantId,
                "user@example.com",
                "Test User",
                TenantUserRole.Viewer,
                DateTimeOffset.UtcNow);

            user.Deactivate();

            // Act
            user.Activate();

            // Assert
            Assert.True(user.IsActive);
        }

        [Fact]
        public void Activate_WhenAlreadyActive_ShouldThrow()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var user = TenantUser.Create(
                tenantId,
                "user@example.com",
                "Test User",
                TenantUserRole.Viewer,
                DateTimeOffset.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                user.Activate());
        }

        [Fact]
        public void UpdateProfile_WithValidName_ShouldUpdateDisplayName()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var user = TenantUser.Create(
                tenantId,
                "user@example.com",
                "Old Name",
                TenantUserRole.Viewer,
                DateTimeOffset.UtcNow);

            // Act
            user.UpdateProfile("New Name");

            // Assert
            Assert.Equal("New Name", user.DisplayName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateProfile_WithInvalidName_ShouldThrow(string? newName)
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var user = TenantUser.Create(
                tenantId,
                "user@example.com",
                "Old Name",
                TenantUserRole.Viewer,
                DateTimeOffset.UtcNow);

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                user.UpdateProfile(newName!));
        }
    }
}