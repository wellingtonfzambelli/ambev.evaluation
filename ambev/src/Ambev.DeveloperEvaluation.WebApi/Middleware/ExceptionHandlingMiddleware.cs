using Ambev.DeveloperEvaluation.WebApi.Common;
using Serilog.Context;
using System.Text.Json;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var correlationId = ResolveCorrelationId(context);
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                using (LogContext.PushProperty("CorrelationId", correlationId))
                using (_logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId }))
                {
                    _logger.LogError(ex, "Unhandled exception");
                }
            }
            else
            {
                _logger.LogError(ex, "Unhandled exception");
            }

            await HandleExceptionAsync(context);
        }
    }

    private static string? ResolveCorrelationId(HttpContext context)
    {
        if (context.Items.TryGetValue(RequireCorrelationIdAttribute.HeaderName, out var value) &&
            value is string correlationIdFromItems &&
            Guid.TryParse(correlationIdFromItems, out _))
        {
            return correlationIdFromItems;
        }

        if (context.Request.Headers.TryGetValue(RequireCorrelationIdAttribute.HeaderName, out var headerValue) &&
            Guid.TryParse(headerValue, out var correlationIdFromHeader))
        {
            return correlationIdFromHeader.ToString();
        }

        return null;
    }

    private static Task HandleExceptionAsync(HttpContext context)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new ApiResponse
        {
            Success = false,
            Message = "An error occurred, please try again later."
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
