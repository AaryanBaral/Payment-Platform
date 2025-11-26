using PaymentPlatform.Domain.Common;

namespace PaymentPlatform.UnitTests.Domain.common
{
    public class MoneyTests
    {
        [Fact]
    public void From_WithValidInput_ShouldCreateInstance()
    {
        // Arrange
        decimal amount = 1000m;
        string currency = "npr";

        // Act
        var money = Money.From(amount, currency);

        // Assert
        Assert.Equal(amount, money.Amount);
        Assert.Equal("NPR", money.Currency); // we uppercased it
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public void From_WithNegativeAmount_ShouldThrow(decimal amount)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Money.From(amount, "NPR"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void From_WithInvalidCurrency_ShouldThrow(string? currency)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => Money.From(100m, currency!));
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldSumAmounts()
    {
        // Arrange
        var m1 = Money.From(100m, "NPR");
        var m2 = Money.From(50m, "NPR");

        // Act
        var result = m1.Add(m2);

        // Assert
        Assert.Equal(150m, result.Amount);
        Assert.Equal("NPR", result.Currency);
    }

    [Fact]
    public void Add_WithDifferentCurrency_ShouldThrow()
    {
        // Arrange
        var m1 = Money.From(100m, "NPR");
        var m2 = Money.From(50m, "USD");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => m1.Add(m2));
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldSubtractAmounts()
    {
        // Arrange
        var m1 = Money.From(100m, "NPR");
        var m2 = Money.From(40m, "NPR");

        // Act
        var result = m1.Subtract(m2);

        // Assert
        Assert.Equal(60m, result.Amount);
        Assert.Equal("NPR", result.Currency);
    }

    [Fact]
    public void Subtract_ResultingNegative_ShouldThrow()
    {
        // Arrange
        var m1 = Money.From(50m, "NPR");
        var m2 = Money.From(100m, "NPR");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => m1.Subtract(m2));
    }

    [Fact]
    public void Multiply_ShouldReturnScaledAmount()
    {
        // Arrange
        var money = Money.From(100m, "NPR");
        decimal factor = 1.5m;

        // Act
        var result = money.Multiply(factor);

        // Assert
        Assert.Equal(150m, result.Amount);
        Assert.Equal("NPR", result.Currency);
    }

    [Fact]
    public void Equals_WithSameAmountAndCurrency_ShouldBeEqual()
    {
        // Arrange
        var m1 = Money.From(100m, "NPR");
        var m2 = Money.From(100m, "npr"); // lower-case but should normalize

        // Act & Assert
        Assert.Equal(m1, m2);
        Assert.True(m1.Equals(m2));
    }

    [Fact]
    public void Equals_WithDifferentAmountOrCurrency_ShouldNotBeEqual()
    {
        // Arrange
        var m1 = Money.From(100m, "NPR");
        var m2 = Money.From(200m, "NPR");
        var m3 = Money.From(100m, "USD");

        // Act & Assert
        Assert.NotEqual(m1, m2);
        Assert.NotEqual(m1, m3);
    }
    }
}