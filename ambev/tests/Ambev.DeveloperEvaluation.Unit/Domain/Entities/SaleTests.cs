using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public sealed class SaleTests
{
    [Fact(DisplayName = "Sale should initialize with defaults")]
    public void Given_NewSale_When_Created_Then_ShouldHaveDefaults()
    {
        // Arrange
        var sale = CreateSale();

        // Act
        var result = sale;

        // Assert
        Assert.Equal(SaleStatus.NotCanceled, result.SaleStatus);
        Assert.Equal(0m, result.TotalAmount);
        Assert.NotEqual(default, result.CreatedAt);
        Assert.Null(result.UpdateAt);
        Assert.Empty(result.Items);
    }

    [Fact(DisplayName = "Sale should add item and increase quantity for same product")]
    public void Given_SameProduct_When_AddedTwice_Then_ShouldIncreaseQuantity()
    {
        // Arrange
        var sale = CreateSale();
        var productId = Guid.NewGuid();

        // Act
        sale.AddItem(productId, "Guitar", 3, 10m);
        sale.AddItem(productId, "Guitar", 1, 10m);

        // Assert
        Assert.Single(sale.Items);
        Assert.Equal(4, sale.Items.First().Quantity);
        Assert.Equal(36m, sale.TotalAmount);
        Assert.NotNull(sale.UpdateAt);
    }

    [Fact(DisplayName = "Sale should replace items and update total")]
    public void Given_Items_When_Replaced_Then_ShouldResetCollection()
    {
        // Arrange
        var sale = CreateSale();
        sale.AddItem(Guid.NewGuid(), "Guitar", 1, 10m);

        var newItems = new[]
        {
            (Guid.NewGuid(), "Bass", 4, 10m),
            (Guid.NewGuid(), "Drums", 2, 20m)
        };

        // Act
        sale.ReplaceItems(newItems);

        // Assert
        Assert.Equal(2, sale.Items.Count);
        Assert.Equal(4, sale.Items.First().Quantity);
        Assert.NotNull(sale.UpdateAt);
    }


    [Fact(DisplayName = "Sale should cancel status and refuse double cancel")]
    public void Given_Sale_When_CancelledTwice_Then_ShouldReturnFalseSecondTime()
    {
        // Arrange
        var sale = CreateSale();

        // Act
        var firstCancel = sale.CancelStatus();
        var secondCancel = sale.CancelStatus();

        // Assert
        Assert.True(firstCancel);
        Assert.False(secondCancel);
        Assert.Equal(SaleStatus.Canceled, sale.SaleStatus);
        Assert.NotNull(sale.UpdateAt);
    }

    private static Sale CreateSale()
    {
        return new Sale
        (
            $"S-{Guid.NewGuid():N}".Substring(0, 8),
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch"
        );
    }
}