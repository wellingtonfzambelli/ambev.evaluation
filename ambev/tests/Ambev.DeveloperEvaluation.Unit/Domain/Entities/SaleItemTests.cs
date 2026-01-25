using Ambev.DeveloperEvaluation.Domain.Entities;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public sealed class SaleItemTests
{
    [Fact(DisplayName = "SaleItem should calculate total without discount for quantity below 4")]
    public void Given_QuantityBelowFour_When_Created_Then_TotalShouldHaveNoDiscount()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var item = new SaleItem(productId, "Guitar", 3, 10m);

        // Assert
        Assert.Equal(0m, item.PercentageDiscount);
        Assert.Equal(30m, item.Total);
    }

    [Fact(DisplayName = "SaleItem should apply 10% discount for quantity 4-9")]
    public void Given_QuantityFour_When_Created_Then_DiscountShouldBeTenPercent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var item = new SaleItem(productId, "Bass", 4, 10m);

        // Assert
        Assert.Equal(0.10m, item.PercentageDiscount);
        Assert.Equal(36m, item.Total);
    }

    [Fact(DisplayName = "SaleItem should apply 20% discount for quantity 10 or more")]
    public void Given_QuantityTen_When_Created_Then_DiscountShouldBeTwentyPercent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var item = new SaleItem(productId, "Drums", 10, 10m);

        // Assert
        Assert.Equal(0.20m, item.PercentageDiscount);
        Assert.Equal(80m, item.Total);
    }

    [Fact(DisplayName = "SaleItem should update discount when quantity increases")]
    public void Given_QuantityIncrease_When_Increased_Then_ShouldRecalculateDiscount()
    {
        // Arrange
        var item = new SaleItem(Guid.NewGuid(), "Microphone", 3, 10m);

        // Act
        item.IncreaseQuantity(1);

        // Assert
        Assert.Equal(4, item.Quantity);
        Assert.Equal(0.10m, item.PercentageDiscount);
        Assert.Equal(36m, item.Total);
    }

    [Fact(DisplayName = "SaleItem should throw when quantity exceeds 20")]
    public void Given_QuantityAboveLimit_When_Created_Then_ShouldThrow()
    {
        // Arrange
        var productId = Guid.NewGuid();

        // Act
        var exception = Record.Exception(() => new SaleItem(productId, "Amp", 21, 10m));

        // Assert
        Assert.IsType<DomainException>(exception);
    }

    [Fact(DisplayName = "SaleItem should throw when increasing quantity exceeds 20")]
    public void Given_QuantityIncreaseAboveLimit_When_Increased_Then_ShouldThrow()
    {
        // Arrange
        var item = new SaleItem(Guid.NewGuid(), "Speaker", 20, 10m);

        // Act
        var exception = Record.Exception(() => item.IncreaseQuantity(1));

        // Assert
        Assert.IsType<DomainException>(exception);
    }
}
