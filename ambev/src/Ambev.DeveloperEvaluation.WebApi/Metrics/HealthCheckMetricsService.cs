using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace Ambev.DeveloperEvaluation.WebApi.Metrics;

public sealed class HealthCheckMetricsService : BackgroundService
{
    private static readonly Gauge HealthCheckStatus = global::Prometheus.Metrics.CreateGauge(
        "ambev_healthcheck_status",
        "Health check status (1 healthy, 0 unhealthy).",
        new GaugeConfiguration { LabelNames = ["name"] });

    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthCheckMetricsService> _logger;

    public HealthCheckMetricsService(
        HealthCheckService healthCheckService,
        ILogger<HealthCheckMetricsService> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
        _logger.LogInformation("Health check metrics worker constructed.");
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Health check metrics worker starting.");
        return base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Health check metrics worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                HealthReport report = await _healthCheckService.CheckHealthAsync(stoppingToken);

                if (report.Entries.Count == 0)
                {
                    _logger.LogWarning("No health checks registered for metrics export.");
                }

                foreach (var entry in report.Entries)
                {
                    var isHealthy = entry.Value.Status == HealthStatus.Healthy;
                    HealthCheckStatus.WithLabels(entry.Key).Set(isHealthy ? 1 : 0);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore shutdown cancellation.
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update health check metrics.");
            }

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}
