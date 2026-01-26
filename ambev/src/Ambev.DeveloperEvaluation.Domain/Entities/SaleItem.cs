using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public sealed class SaleItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal PercentageDiscount { get; private set; }
    public decimal Total => Quantity * UnitPrice * (1 - PercentageDiscount);

    public Product Product { get; private set; }

    public SaleItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        DiscountRules.ValidateQuantity(quantity);

        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
        PercentageDiscount = DiscountRules.CalculatePercentageDiscount(quantity);
    }

    public void IncreaseQuantity(int quantity)
    {
        var newQuantity = Quantity + quantity;
        DiscountRules.ValidateQuantity(newQuantity);

        Quantity = newQuantity;
        PercentageDiscount = DiscountRules.CalculatePercentageDiscount(Quantity);
    }
}