using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Query for listing sales.
/// </summary>
public sealed record ListSalesQuery : IRequest<IReadOnlyList<ListSalesResult>>;
