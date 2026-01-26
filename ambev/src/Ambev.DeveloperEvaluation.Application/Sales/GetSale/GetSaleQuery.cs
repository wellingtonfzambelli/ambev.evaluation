using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public sealed record GetSaleQuery : IRequest<GetSaleResult>
{
    public Guid Id { get; }

    public GetSaleQuery(Guid id)
    {
        Id = id;
    }
}