using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemMonitor.Common;
using SystemMonitor.Infrastructure;

namespace SystemMonitor.Plugins
{
    public class PluginManager : IPluginManager
    {
        private readonly IEnumerable<IMonitorPlugin> _plugins;

        public PluginManager(IEnumerable<IMonitorPlugin> plugins)
        {
            _plugins = plugins ?? throw new ArgumentNullException(nameof(plugins));
        }

        public async Task NotifyPluginsAsync(SystemMetrics metrics)
        {
            foreach (var plugin in _plugins)
            {
                try
                {
                    await plugin.ProcessMetricsAsync(metrics);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in plugin {plugin.GetType().Name}: {ex.Message}");
                }
            }
        }
    }
}
