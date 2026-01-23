using Ambev.DeveloperEvaluation.WebApi.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware;

public sealed class RequireCorrelationIdAttribute : Attribute, IAsyncActionFilter
{
    public const string HeaderName = "correlationId";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var headerValue) ||
            !Guid.TryParse(headerValue, out var correlationId))
        {
            context.Result = new BadRequestObjectResult(new ApiResponse
            {
                Success = false,
                Message = "correlationId header is required and must be a valid GUID."
            });
            return;
        }

        context.HttpContext.Items[HeaderName] = correlationId.ToString();
        context.HttpContext.Response.Headers[HeaderName] = correlationId.ToString();

        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RequireCorrelationIdAttribute>>();
        using (logger.BeginScope(new Dictionary<string, object> { ["CorrelationId"] = correlationId.ToString() }))
        using (LogContext.PushProperty("CorrelationId", correlationId.ToString()))
        {
            await next();
        }
    }
}