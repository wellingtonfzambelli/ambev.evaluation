using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public sealed class SaleItemTests
{
    [Theory(DisplayName = "SaleItem should apply correct discounts by quantity")]
    [InlineData(3, 0.0, 30.0)]
    [InlineData(4, 0.10, 36.0)]
    [InlineData(9, 0.10, 81.0)]
    [InlineData(10, 0.20, 80.0)]
    [InlineData(20, 0.20, 160.0)]
    public void Given_Quantity_When_Created_Then_ShouldApplyCorrectDiscount(int quantity, decimal expectedDiscount, decimal expectedTotal)
    {
        // Arrange
        var productId = Guid.NewGuid();
        const decimal unitPrice = 10m;

        // Act
        var item = new SaleItem(productId, "Drums", quantity, unitPrice);

        // Assert
        Assert.Equal(expectedDiscount, item.PercentageDiscount);
        Assert.Equal(expectedTotal, item.Total);
    }

    [Theory(DisplayName = "SaleItem should update discount when quantity increases")]
    [InlineData(3, 1, 4, 0.10, 36.0)]
    [InlineData(9, 1, 10, 0.20, 80.0)]
    public void Given_QuantityIncrease_When_Increased_Then_ShouldRecalculateDiscount(
        int initialQuantity,
        int increaseBy,
        int expectedQuantity,
        decimal expectedDiscount,
        decimal expectedTotal)
    {
        // Arrange
        const decimal unitPrice = 10m;
        var item = new SaleItem(Guid.NewGuid(), "Microphone", initialQuantity, unitPrice);

        // Act
        item.IncreaseQuantity(increaseBy);

        // Assert
        Assert.Equal(expectedQuantity, item.Quantity);
        Assert.Equal(expectedDiscount, item.PercentageDiscount);
        Assert.Equal(expectedTotal, item.Total);
    }

    [Theory(DisplayName = "SaleItem should throw when quantity exceeds 20")]
    [InlineData(21)]
    [InlineData(25)]
    public void Given_QuantityAboveLimit_When_Created_Then_ShouldThrow(int quantity)
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var exception = Record.Exception(() => new SaleItem(productId, "Amp", quantity, 10m));

        // Assert
        Assert.IsType<DomainException>(exception);
    }

    [Theory(DisplayName = "SaleItem should throw when increasing quantity exceeds 20")]
    [InlineData(20, 1)]
    [InlineData(19, 2)]
    public void Given_QuantityIncreaseAboveLimit_When_Increased_Then_ShouldThrow(int initialQuantity, int increaseBy)
    {
        // Arrange
        var item = new SaleItem(Guid.NewGuid(), "Speaker", initialQuantity, 10m);

        // Act
        var exception = Record.Exception(() => item.IncreaseQuantity(increaseBy));

        // Assert
        Assert.IsType<DomainException>(exception);
    }
}
