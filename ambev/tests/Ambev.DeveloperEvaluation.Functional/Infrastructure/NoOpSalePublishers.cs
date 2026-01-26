using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.EnqueueSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.Functional.Infrastructure;

internal sealed class NoOpSaleCreatedPublisher : ISaleCreatedPublisher
{
    public Task PublishAsync(Guid saleId, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal sealed class NoOpSaleUpdatedPublisher : ISaleUpdatedPublisher
{
    public Task PublishAsync(Guid saleId, CancellationToken cancellationToken) => Task.CompletedTask;
}

internal sealed class NoOpSaleCancelledPublisher : ISaleCancelledPublisher
{
    public Task PublishAsync(Guid saleId, CancellationToken cancellationToken) => Task.CompletedTask;
}
