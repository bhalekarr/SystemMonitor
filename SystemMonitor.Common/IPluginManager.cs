using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Common
{
    public interface IPluginManager
    {
        Task NotifyPluginsAsync(SystemMetrics metrics);
    }
}
