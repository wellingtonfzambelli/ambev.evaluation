namespace Ambev.DeveloperEvaluation.Application.Common;

public interface ICorrelationContext
{
    string? CorrelationId { get; }
}
