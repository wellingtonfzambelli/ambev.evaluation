namespace Ambev.DeveloperEvaluation.Application.Sales.EnqueueSale;

public interface ISaleCreatedPublisher
{
    Task PublishAsync(Guid saleId, CancellationToken cancellationToken);
}
