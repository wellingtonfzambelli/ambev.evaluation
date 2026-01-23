using System.Text.Json;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using FluentValidation;
using Serilog.Context;

namespace Ambev.DeveloperEvaluation.WebApi.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            var correlationId = ResolveCorrelationId(context);
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                using (LogContext.PushProperty("CorrelationId", correlationId))
                {
                }
            }

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

        private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var correlationId = ResolveCorrelationId(context);
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                context.Response.Headers[RequireCorrelationIdAttribute.HeaderName] = correlationId;
            }

            var response = new ApiResponse
            {
                Success = false,
                Message = "Validation Failed",
                Errors = exception.Errors
                    .Select(error => (ValidationErrorDetail)error)
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
