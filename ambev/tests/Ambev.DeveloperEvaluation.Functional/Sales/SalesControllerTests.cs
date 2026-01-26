using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Functional.Infrastructure;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Sales;

public sealed class SalesControllerTests
{
    [Fact]
    public async Task CreateSale_ReturnsAccepted_WhenValid()
    {
        // Arrange
        using var factory = new SalesApiFactory();
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        await SeedAsync(factory, context =>
        {
            context.Users.Add(new User
            {
                Id = customerId,
                Username = "Test Customer",
                Email = "customer@example.com",
                Password = "P@ssword1",
                Phone = "(11) 99999-9999",
                Role = UserRole.Customer,
                Status = UserStatus.Active
            });
            context.Branches.Add(new Branch("Main Branch") { Id = branchId });
            context.Products.Add(new Product("Soda", 10m) { Id = productId });
        });

        using var client = CreateClient(factory);
        var request = new CreateSaleRequest
        (
            SaleNumber: "SALE-0001",
            CustomerId: customerId,
            BranchId: branchId,
            Items: new[]
            {
                new CreateSaleItemRequest(productId, 2, 10m)
            }
        );

        // Act
        var response = await client.PostAsJsonAsync("/api/Sales", request);

        // Assert
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponse>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.Equal("Sale creation queued successfully", payload.Message);

        await AssertSaleCountAsync(factory, 1);
    }

    [Fact]
    public async Task GetSale_ReturnsOk_WhenSaleExists()
    {
        // Arrange
        using var factory = new SalesApiFactory();
        var saleId = Guid.NewGuid();

        await SeedAsync(factory, context =>
        {
            var sale = BuildSale(saleId);
            context.Sales.Add(sale);
        });

        using var client = CreateClient(factory);

        // Act
        var response = await client.GetAsync($"/api/Sales/{saleId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponseWithData<ApiResponseWithData<GetSaleResponse>>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.NotNull(payload.Data);
        Assert.True(payload.Data!.Success);
        Assert.NotNull(payload.Data.Data);
        Assert.Equal(saleId, payload.Data.Data!.SaleId);
    }

    [Fact]
    public async Task ListSales_ReturnsSales()
    {
        // Arrange
        using var factory = new SalesApiFactory();
        await SeedAsync(factory, context =>
        {
            context.Sales.Add(BuildSale(Guid.NewGuid(), "SALE-1001"));
            context.Sales.Add(BuildSale(Guid.NewGuid(), "SALE-1002"));
        });

        using var client = CreateClient(factory);

        // Act
        var response = await client.GetAsync("/api/Sales");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(content);
        var root = document.RootElement;
        Assert.True(root.TryGetProperty("success", out var successElement));
        Assert.True(successElement.GetBoolean());

        Assert.True(root.TryGetProperty("data", out var dataElement));
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        IReadOnlyCollection<ListSalesResponse> sales;

        if (dataElement.ValueKind == JsonValueKind.Array)
        {
            sales = JsonSerializer.Deserialize<List<ListSalesResponse>>(
                dataElement.GetRawText(),
                options) ?? [];
        }
        else if (dataElement.ValueKind == JsonValueKind.Object)
        {
            JsonElement arrayElement;
            if (dataElement.TryGetProperty("$values", out arrayElement) ||
                dataElement.TryGetProperty("values", out arrayElement) ||
                dataElement.TryGetProperty("items", out arrayElement))
            {
                if (arrayElement.ValueKind == JsonValueKind.Array)
                {
                    sales = JsonSerializer.Deserialize<List<ListSalesResponse>>(
                        arrayElement.GetRawText(),
                        options) ?? [];
                }
                else
                {
                    sales = [];
                }
            }
            else
            {
                Assert.True(false, $"Unexpected data payload: {dataElement.GetRawText()}");
                sales = [];
            }
        }
        else
        {
            Assert.True(false, $"Unexpected data payload: {dataElement.GetRawText()}");
            sales = [];
        }

        Assert.Equal(2, sales.Count);
    }

    [Fact]
    public async Task ReplaceSaleItems_ReturnsOk_WhenSaleIsActive()
    {
        // Arrange
        using var factory = new SalesApiFactory();
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        await SeedAsync(factory, context =>
        {
            context.Sales.Add(BuildSale(saleId, "SALE-2001"));
            context.Products.Add(new Product("Updated Item", 12m) { Id = productId });
        });

        using var client = CreateClient(factory);
        var request = new UpdateSaleRequest
        {
            Items = new[]
            {
                new UpdateSaleItemRequest
                {
                    ProductId = productId,
                    Quantity = 4,
                    UnitPrice = 12m
                }
            }
        };

        // Act
        var response = await client.PatchAsJsonAsync($"/api/Sales/{saleId}/SaleItem", request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<ApiResponseWithData<ApiResponseWithData<UpdateSaleResponse>>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        Assert.NotNull(payload.Data);
        Assert.True(payload.Data!.Success);
        Assert.NotNull(payload.Data.Data);
        Assert.Equal(saleId, payload.Data.Data!.SaleId);
    }

    [Fact]
    public async Task CancelSale_ReturnsOk_WhenSaleExists()
    {
        // Arrange
        using var factory = new SalesApiFactory();
        var saleId = Guid.NewGuid();

        await SeedAsync(factory, context =>
        {
            context.Sales.Add(BuildSale(saleId, "SALE-3001"));
        });

        using var client = CreateClient(factory);

        // Act
        var response = await client.PatchAsync($"/api/Sales/{saleId}/cancel", content: null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var payload = JsonSerializer.Deserialize<ApiResponse>(
            content,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(payload);
        Assert.True(payload!.Success);
        if (!string.IsNullOrWhiteSpace(payload.Message))
        {
            Assert.Equal("Sale cancelled successfully", payload.Message);
        }
    }

    private static HttpClient CreateClient(SalesApiFactory factory)
    {
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(
            RequireCorrelationIdAttribute.HeaderName,
            Guid.NewGuid().ToString());
        return client;
    }

    private static Sale BuildSale(Guid saleId, string saleNumber = "SALE-0002")
    {
        var sale = new Sale
        (
            saleNumber,
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Test Customer",
            Guid.NewGuid(),
            "Test Branch"
        );
        sale.Id = saleId;
        sale.AddItem(Guid.NewGuid(), "Item", 1, 10m);
        return sale;
    }

    private static async Task SeedAsync(SalesApiFactory factory, Action<DefaultContext> seed)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        await context.Database.EnsureCreatedAsync();
        seed(context);
        await context.SaveChangesAsync();
    }

    private static async Task AssertSaleCountAsync(SalesApiFactory factory, int expected)
    {
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
        var count = await context.Sales.CountAsync();
        Assert.Equal(expected, count);
    }
}