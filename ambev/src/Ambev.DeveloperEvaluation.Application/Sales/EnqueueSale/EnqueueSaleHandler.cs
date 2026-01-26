using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.EnqueueSale;

public sealed class EnqueueSaleHandler : IRequestHandler<EnqueueSaleCommand>
{
    private readonly ISaleCreatedPublisher _publisher;

    public EnqueueSaleHandler(ISaleCreatedPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task Handle(EnqueueSaleCommand request, CancellationToken cancellationToken)
    {
        await _publisher.PublishAsync(request.SaleId, cancellationToken);
    }
}
