using System.Threading.RateLimiting;
using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.EnqueueSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.HealthChecks;
using Ambev.DeveloperEvaluation.Common.Logging;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");

            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.AddDefaultLogging();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetSection("Redis").GetValue<string>("Configuration");
            });
            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        "global",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 2,
                            Window = TimeSpan.FromSeconds(10),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        }));
            });

            builder.AddBasicHealthChecks();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your token."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddDbContext<DefaultContext>(options =>
                options.UseNpgsql(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("Ambev.DeveloperEvaluation.ORM")
                )
            );

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.RegisterDependencies();

            builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblies(
                    typeof(ApplicationLayer).Assembly,
                    typeof(Program).Assembly
                );
            });

            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddScoped<ICorrelationContext, CorrelationContext>();

            // Messaging - RabbitMQ - MassTransit
            builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
            builder.Services.AddScoped<ISaleCreatedPublisher, SaleCreatedPublisher>();
            builder.Services.AddScoped<ISaleUpdatedPublisher, SaleUpdatedPublisher>();
            builder.Services.AddScoped<ISaleCancelledPublisher, SaleCancelledPublisher>();
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<SaleCreatedConsumer>();
                x.AddConsumer<SaleUpdatedConsumer>();
                x.AddConsumer<SaleCancelledConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var settings = builder.Configuration.GetSection("RabbitMq").Get<RabbitMqSettings>()
                        ?? new RabbitMqSettings();

                    cfg.Host(settings.HostName, h =>
                    {
                        h.Username(settings.UserName);
                        h.Password(settings.Password);
                    });

                    cfg.Message<SaleCreatedMessage>(m => m.SetEntityName("x.sale.created"));
                    cfg.Publish<SaleCreatedMessage>(p => p.ExchangeType = ExchangeType.Direct);
                    cfg.Message<SaleUpdatedMessage>(m => m.SetEntityName("x.sale.updated"));
                    cfg.Publish<SaleUpdatedMessage>(p => p.ExchangeType = ExchangeType.Direct);
                    cfg.Message<SaleCancelledMessage>(m => m.SetEntityName("x.sale.cancelled"));
                    cfg.Publish<SaleCancelledMessage>(p => p.ExchangeType = ExchangeType.Direct);

                    cfg.ReceiveEndpoint("q.sale.created", e =>
                    {
                        e.Bind("x.sale.created", s =>
                        {
                            s.RoutingKey = "rk.sale.created";
                            s.ExchangeType = ExchangeType.Direct;
                        });
                        e.ConfigureConsumer<SaleCreatedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("q.sale.updated", e =>
                    {
                        e.Bind("x.sale.updated", s =>
                        {
                            s.RoutingKey = "rk.sale.updated";
                            s.ExchangeType = ExchangeType.Direct;
                        });
                        e.ConfigureConsumer<SaleUpdatedConsumer>(context);
                    });

                    cfg.ReceiveEndpoint("q.sale.cancelled", e =>
                    {
                        e.Bind("x.sale.cancelled", s =>
                        {
                            s.RoutingKey = "rk.sale.cancelled";
                            s.ExchangeType = ExchangeType.Direct;
                        });
                        e.ConfigureConsumer<SaleCancelledConsumer>(context);
                    });
                });
            });

            var app = builder.Build();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<ValidationExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBasicHealthChecks();

            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
