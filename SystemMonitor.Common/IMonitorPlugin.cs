using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Common
{
    public interface IMonitorPlugin
    {
        Task ProcessMetricsAsync(SystemMetrics metrics);
    }
}
