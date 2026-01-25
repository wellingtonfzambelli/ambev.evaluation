using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.EnqueueSale;

public sealed class EnqueueSaleCommand : IRequest
{
    public Guid SaleId { get; set; }
}