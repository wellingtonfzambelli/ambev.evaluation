namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Request model for replacing sale items.
/// </summary>
public sealed class UpdateSaleRequest
{
    public IReadOnlyCollection<UpdateSaleItemRequest> Items { get; set; }
        = Array.Empty<UpdateSaleItemRequest>();
}

public sealed class UpdateSaleItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
