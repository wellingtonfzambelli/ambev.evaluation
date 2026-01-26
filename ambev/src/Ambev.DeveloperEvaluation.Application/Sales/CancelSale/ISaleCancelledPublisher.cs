namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public interface ISaleCancelledPublisher
{
    Task PublishAsync(Guid saleId, CancellationToken cancellationToken);
}
