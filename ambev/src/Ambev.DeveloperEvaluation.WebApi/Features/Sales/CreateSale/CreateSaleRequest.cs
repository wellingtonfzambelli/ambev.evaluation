namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public sealed record CreateSaleRequest
(
    string SaleNumber,
    Guid UserId,
    Guid BranchId,
    IReadOnlyCollection<CreateSaleItemRequest> Items
);

public sealed record CreateSaleItemRequest
(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);