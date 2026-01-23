namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;

public sealed class ListProductsResponse
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
