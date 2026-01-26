using System.Diagnostics;
using Prometheus;

namespace Ambev.DeveloperEvaluation.WebApi.Metrics;

public sealed class ProcessMetricsService : BackgroundService
{
    private static readonly Gauge ProcessCpuPercent = global::Prometheus.Metrics.CreateGauge(
        "ambev_process_cpu_percent",
        "Process CPU usage percentage.");

    private static readonly Gauge ProcessWorkingSetBytes = global::Prometheus.Metrics.CreateGauge(
        "ambev_process_working_set_bytes",
        "Process working set in bytes.");

    private readonly ILogger<ProcessMetricsService> _logger;
    private TimeSpan _lastTotalProcessorTime;
    private DateTime _lastSampleTimeUtc;

    public ProcessMetricsService(ILogger<ProcessMetricsService> logger)
    {
        _logger = logger;
        using var process = Process.GetCurrentProcess();
        _lastTotalProcessorTime = process.TotalProcessorTime;
        _lastSampleTimeUtc = DateTime.UtcNow;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var process = Process.GetCurrentProcess();
                var now = DateTime.UtcNow;
                var totalCpu = process.TotalProcessorTime;
                var cpuDeltaMs = (totalCpu - _lastTotalProcessorTime).TotalMilliseconds;
                var timeDeltaMs = (now - _lastSampleTimeUtc).TotalMilliseconds;

                if (timeDeltaMs > 0)
                {
                    var cpuPercent = (cpuDeltaMs / (timeDeltaMs * Environment.ProcessorCount)) * 100;
                    if (cpuPercent < 0)
                    {
                        cpuPercent = 0;
                    }

                    ProcessCpuPercent.Set(cpuPercent);
                }

                ProcessWorkingSetBytes.Set(process.WorkingSet64);

                _lastTotalProcessorTime = totalCpu;
                _lastSampleTimeUtc = now;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update process metrics.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
