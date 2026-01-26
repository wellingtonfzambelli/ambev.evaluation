namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Represents a sale item in the list response.
/// </summary>
public sealed class ListSalesResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public IReadOnlyCollection<ListSalesItemResult> Items { get; set; }
        = Array.Empty<ListSalesItemResult>();
}

public sealed class ListSalesItemResult
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
}
