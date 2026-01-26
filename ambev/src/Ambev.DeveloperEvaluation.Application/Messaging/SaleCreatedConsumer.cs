using Ambev.DeveloperEvaluation.Application.Common;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Messaging;

public sealed class SaleCreatedConsumer : IConsumer<SaleCreatedMessage>
{
    private readonly ICorrelationContext _correlationContext;
    private readonly ILogger<SaleCreatedConsumer> _logger;

    public SaleCreatedConsumer
    (
        ICorrelationContext correlationContext,
        ILogger<SaleCreatedConsumer> logger
    )
    {
        _correlationContext = correlationContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SaleCreatedMessage> context)
    {
        SaleCreatedMessage message = context.Message;

        var correlationId = context.Headers.TryGetHeader("correlationId", out var headerValue)
            ? headerValue?.ToString()
            : _correlationContext.CorrelationId;

        _logger.LogInformation(
            "SaleCreated consumed. SaleId: {SaleId}.",
            message.SaleId);

        await Task.CompletedTask;
    }
}