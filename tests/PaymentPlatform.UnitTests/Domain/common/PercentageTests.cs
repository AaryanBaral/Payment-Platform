using PaymentPlatform.Domain.Common;

namespace PaymentPlatform.UnitTests.Domain.common
{

    public class PercentageTests
    {
        [Fact]
        public void From_WithValidValue_ShouldCreateInstance()
        {
            // Arrange
            decimal value = 80m;

            // Act
            var percentage = Percentage.From(value);

            // Assert
            Assert.Equal(value, percentage.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        [InlineData(100)]
        [InlineData(150)]
        public void From_WithInvalidValue_ShouldThrow(decimal value)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Percentage.From(value));
        }
        [Fact]
        public void Equals_WithSameValue_ShouldBeEqual()
        {
            // Arrange
            var p1 = Percentage.From(80m);
            var p2 = Percentage.From(80m);

            // Act
            var areEqual = p1.Equals(p2);

            // Assert
            Assert.True(areEqual);
            Assert.Equal(p1, p2);
        }

        [Fact]
        public void Equals_WithDifferentValue_ShouldNotBeEqual()
        {
            // Arrange
            var p1 = Percentage.From(80m);
            var p2 = Percentage.From(20m);

            // Act
            var areEqual = p1.Equals(p2);

            // Assert
            Assert.False(areEqual);
            Assert.NotEqual(p1, p2);
        }

        [Fact]
        public void AsFraction_ShouldReturnValueDividedBy100()
        {
            // Arrange
            var percentage = Percentage.From(25m);

            // Act
            var fraction = percentage.AsFraction();

            // Assert
            Assert.Equal(0.25m, fraction);
        }

    }
}