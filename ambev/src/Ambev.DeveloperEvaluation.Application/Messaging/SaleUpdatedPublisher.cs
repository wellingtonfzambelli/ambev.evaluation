using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Messaging;

public sealed class SaleUpdatedPublisher : ISaleUpdatedPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<SaleUpdatedPublisher> _logger;

    public SaleUpdatedPublisher
    (
        IPublishEndpoint publishEndpoint,
        ICorrelationContext correlationContext,
        ILogger<SaleUpdatedPublisher> logger
    )
    {
        _publishEndpoint = publishEndpoint;
        _correlationContext = correlationContext;
        _logger = logger;
    }

    public async Task PublishAsync(Guid saleId, CancellationToken cancellationToken)
    {
        var publishTask = _publishEndpoint.Publish(
            new SaleUpdatedMessage
            {
                SaleId = saleId
            },
            context =>
            {
                context.SetRoutingKey("rk.sale.updated");
                if (!string.IsNullOrWhiteSpace(_correlationContext.CorrelationId))
                {
                    context.Headers.Set("correlationId", _correlationContext.CorrelationId);
                }
            },
            cancellationToken);

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(TimeSpan.FromSeconds(10));
        await publishTask.WaitAsync(timeoutCts.Token);

        _logger.LogInformation(
            "SaleUpdated published. CorrelationId: {CorrelationId}",
            _correlationContext.CorrelationId);
    }
}
