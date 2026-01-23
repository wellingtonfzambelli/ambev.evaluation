namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Request model for getting a sale by ID.
/// </summary>
public sealed class GetSaleRequest
{
    public Guid Id { get; set; }
}
