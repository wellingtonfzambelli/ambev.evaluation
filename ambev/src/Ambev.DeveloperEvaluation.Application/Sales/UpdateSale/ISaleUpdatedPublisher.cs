namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public interface ISaleUpdatedPublisher
{
    Task PublishAsync(Guid saleId, CancellationToken cancellationToken);
}
