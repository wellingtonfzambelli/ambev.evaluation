using System.Threading.RateLimiting;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.EnqueueSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Ambev.DeveloperEvaluation.Functional.Infrastructure;

internal sealed class SalesApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"sales-tests-{Guid.NewGuid():N}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "test-secret-key-1234567890"
            };
            config.AddInMemoryCollection(settings);
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<DefaultContext>>();
            services.AddDbContext<DefaultContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.RemoveAll<IDistributedCache>();
            services.AddDistributedMemoryCache();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName,
                    _ => { });

            services.AddAuthorization();

            services.RemoveAll<ISaleCreatedPublisher>();
            services.RemoveAll<ISaleUpdatedPublisher>();
            services.RemoveAll<ISaleCancelledPublisher>();
            services.AddScoped<ISaleCreatedPublisher, NoOpSaleCreatedPublisher>();
            services.AddScoped<ISaleUpdatedPublisher, NoOpSaleUpdatedPublisher>();
            services.AddScoped<ISaleCancelledPublisher, NoOpSaleCancelledPublisher>();

            var hostedServices = services
                .Where(descriptor =>
                    descriptor.ServiceType == typeof(IHostedService) &&
                    string.Equals(
                        descriptor.ImplementationType?.Name,
                        "MassTransitHostedService",
                        StringComparison.Ordinal))
                .ToList();

            foreach (var descriptor in hostedServices)
            {
                services.Remove(descriptor);
            }

            services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        "tests",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 1000,
                            Window = TimeSpan.FromSeconds(1),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        }));
            });
        });
    }
}
