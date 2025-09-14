
using Microsoft.Extensions.Logging;

namespace SystemMonitor.Common
{
    public class PluginManager : IPluginManager
    {
        private readonly IEnumerable<IMonitorPlugin> _plugins;
        private readonly ILogger<PluginManager> _logger;

        public PluginManager(IEnumerable<IMonitorPlugin> plugins, ILogger<PluginManager> logger)
        {
            _plugins = plugins ?? throw new ArgumentNullException(nameof(plugins));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task NotifyPluginsAsync(SystemMetrics metrics)
        {
            foreach (var plugin in _plugins)
            {
                try
                {
                    _logger.LogInformation("Processing plugin {PluginName}", plugin.GetType().Name);
                    await plugin.ProcessMetricsAsync(metrics);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in plugin {PluginName}", plugin.GetType().Name);
                }
            }
        }
    }
}
