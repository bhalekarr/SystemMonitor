using SystemMonitor.Common;

namespace SystemMonitor.Plugins
{
    public interface IPluginManager
    {
        Task NotifyPluginsAsync(SystemMetrics metrics);
    }
}
