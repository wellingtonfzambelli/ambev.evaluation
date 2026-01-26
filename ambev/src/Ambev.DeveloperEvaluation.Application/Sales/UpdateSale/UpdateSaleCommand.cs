using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Command for replacing sale items.
/// </summary>
public sealed class UpdateSaleCommand : IRequest<UpdateSaleResult>
{
    public Guid Id { get; set; }
    public IReadOnlyCollection<UpdateSaleItemCommand> Items { get; init; }
        = Array.Empty<UpdateSaleItemCommand>();
}

public sealed record UpdateSaleItemCommand
(
    Guid ProductId,
    int Quantity,
    decimal UnitPrice
);
