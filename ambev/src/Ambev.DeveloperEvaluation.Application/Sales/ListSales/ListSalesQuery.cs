using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public sealed record ListSalesQuery : IRequest<IReadOnlyList<ListSalesResult>>;
