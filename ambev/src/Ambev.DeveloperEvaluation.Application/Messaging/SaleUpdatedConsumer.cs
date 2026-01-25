using Ambev.DeveloperEvaluation.Application.Common;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Messaging;

public sealed class SaleUpdatedConsumer : IConsumer<SaleUpdatedMessage>
{
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<SaleUpdatedConsumer> _logger;

    public SaleUpdatedConsumer
    (
        ICorrelationContext correlationContext,
        ILogger<SaleUpdatedConsumer> logger
    )
    {
        _correlationContext = correlationContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SaleUpdatedMessage> context)
    {
        SaleUpdatedMessage message = context.Message;

        var correlationId = context.Headers.TryGetHeader("correlationId", out var headerValue)
            ? headerValue?.ToString()
            : _correlationContext.CorrelationId;

        _logger.LogInformation(
            "SaleUpdated consumed. SaleId: {SaleId}.",
            message.SaleId);

        await Task.CompletedTask;
    }
}