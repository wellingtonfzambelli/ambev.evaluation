namespace Ambev.DeveloperEvaluation.Domain.Services;

public static class DiscountRules
{
    public static void ValidateQuantity(int quantity)
    {
        if (quantity > 20)
            throw new DomainException("Maximum of 20 items allowed.");
    }

    public static decimal CalculatePercentageDiscount(int quantity)
    {
        if (quantity >= 10) return 0.20m;
        if (quantity >= 4) return 0.10m;

        return 0m;
    }
}