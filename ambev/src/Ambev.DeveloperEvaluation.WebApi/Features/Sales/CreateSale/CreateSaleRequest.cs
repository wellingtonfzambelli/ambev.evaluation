namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public sealed record CreateSaleRequest
(
    string SaleNumber,
    Guid CustomerId,
    Guid BranchId,
    IReadOnlyCollection<CreateSaleItemRequest> Items
);

public sealed record CreateSaleItemRequest
(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);