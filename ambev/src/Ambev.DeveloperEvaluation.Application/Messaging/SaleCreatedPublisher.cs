using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.EnqueueSale;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Messaging;

public sealed class SaleCreatedPublisher : ISaleCreatedPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<SaleCreatedPublisher> _logger;

    public SaleCreatedPublisher
    (
        IPublishEndpoint publishEndpoint,
        ICorrelationContext correlationContext,
        ILogger<SaleCreatedPublisher> logger
    )
    {
        _publishEndpoint = publishEndpoint;
        _correlationContext = correlationContext;
        _logger = logger;
    }

    public async Task PublishAsync(Guid saleId, CancellationToken cancellationToken)
    {
        var publishTask = _publishEndpoint.Publish(
            new SaleCreatedMessage
            {
                SaleId = saleId
            },
            context =>
            {
                context.SetRoutingKey("rk.sale.created");
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
            "SaleCreated published. CorrelationId: {CorrelationId}",
            _correlationContext.CorrelationId);
    }
}
