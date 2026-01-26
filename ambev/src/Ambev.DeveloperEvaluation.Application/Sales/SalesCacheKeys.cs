namespace Ambev.DeveloperEvaluation.Application.Sales;

public static class SalesCacheKeys
{
    public const string All = "sales:all";

    public static string GetById(Guid id) => $"sales:{id}";
    public static string Idempotency(string correlationId) => $"sales:idempotency:{correlationId}";
}
