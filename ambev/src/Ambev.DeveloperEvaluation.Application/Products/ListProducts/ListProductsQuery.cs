using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public sealed record ListProductsQuery : IRequest<IReadOnlyList<ListProductsResult>>;
