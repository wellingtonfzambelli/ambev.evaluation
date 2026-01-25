using Ambev.DeveloperEvaluation.Application.Common;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Messaging;

public sealed class SaleCancelledConsumer : IConsumer<SaleCancelledMessage>
{
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<SaleCancelledConsumer> _logger;

    public SaleCancelledConsumer
    (
        ICorrelationContext correlationContext,
        ILogger<SaleCancelledConsumer> logger
    )
    {
        _correlationContext = correlationContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SaleCancelledMessage> context)
    {
        SaleCancelledMessage message = context.Message;

        var correlationId = context.Headers.TryGetHeader("correlationId", out var headerValue)
            ? headerValue?.ToString()
            : _correlationContext.CorrelationId;

        _logger.LogInformation(
            "SaleCancelled consumed. SaleId: {SaleId}.",
            message.SaleId);

        await Task.CompletedTask;
    }
}
