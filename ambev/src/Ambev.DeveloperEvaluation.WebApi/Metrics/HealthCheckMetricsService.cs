using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;

namespace Ambev.DeveloperEvaluation.WebApi.Metrics;

public sealed class HealthCheckMetricsService : BackgroundService
{
    private static readonly Gauge HealthCheckStatus = Metrics.CreateGauge(
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
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                HealthReport report = await _healthCheckService.CheckHealthAsync(stoppingToken);

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
