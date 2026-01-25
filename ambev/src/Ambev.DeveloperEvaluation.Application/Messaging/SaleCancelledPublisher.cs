using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Messaging;

public sealed class SaleCancelledPublisher : ISaleCancelledPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<SaleCancelledPublisher> _logger;

    public SaleCancelledPublisher
    (
        IPublishEndpoint publishEndpoint,
        ICorrelationContext correlationContext,
        ILogger<SaleCancelledPublisher> logger
    )
    {
        _publishEndpoint = publishEndpoint;
        _correlationContext = correlationContext;
        _logger = logger;
    }

    public async Task PublishAsync(Guid saleId, CancellationToken cancellationToken)
    {
        var publishTask = _publishEndpoint.Publish(
            new SaleCancelledMessage
            {
                SaleId = saleId
            },
            context =>
            {
                context.SetRoutingKey("rk.sale.cancelled");
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
            "SaleCancelled published. SaleId: {SaleId}",
            saleId);
    }
}
