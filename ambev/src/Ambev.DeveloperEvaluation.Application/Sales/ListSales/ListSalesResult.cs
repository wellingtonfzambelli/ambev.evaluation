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
    public Guid BranchId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}
