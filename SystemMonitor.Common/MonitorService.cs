using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SystemMonitor.Common
{
    public class MonitorService : IMonitorService
    {
        private readonly IConfiguration _configuration;
        private readonly ISystemMetricsCollector _metricsCollector;
        private readonly IPluginManager _pluginManager;
        private readonly ILogger<MonitorService> _logger;

        public MonitorService(IConfiguration configuration,
            ISystemMetricsCollector metricsCollector,
            IPluginManager pluginManager,
            ILogger<MonitorService> logger
            )
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _metricsCollector = metricsCollector ?? throw new ArgumentNullException(nameof(metricsCollector));
            _pluginManager = pluginManager ?? throw new ArgumentNullException(nameof(pluginManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StartMonitoringAsync(CancellationToken cancellationToken)
        {
            int intervalSeconds = _configuration.GetValue<int>("Monitoring:IntervalSeconds");
            if (intervalSeconds <= 0)
            {
                _logger.LogError("Invalid Monitoring:IntervalSeconds value in json configuration");
                throw new InvalidOperationException("Monitoring:IntervalSeconds must be greater than 0");
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var metrics = await _metricsCollector.CollectMetricsAsync(cancellationToken);
                    DisplayMetrics(metrics);
                    await _pluginManager.NotifyPluginsAsync(metrics);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error collecting or processing metrics");
                }

                await Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationToken);
            }
        }

        private void DisplayMetrics(SystemMetrics metrics)
        {
            _logger.LogInformation(
            "System Metrics: System CPU Usage={CpuUsage:F2}%, System Private RAM Usage={RamUsed:F2}/{RamTotal:F2}MB, System Disk Usage={DiskUsed:F2}/{DiskTotal:F2}MB",
            metrics.CpuUsage, metrics.RamUsed, metrics.RamTotal, metrics.DiskUsed, metrics.DiskTotal);
        }
    }
}
