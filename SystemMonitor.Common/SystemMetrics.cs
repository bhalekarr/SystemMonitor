using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Common
{
    public class SystemMetrics
    {
        public double CpuUsage { get; set; }
        public long RamUsed { get; set; }
        public long RamTotal { get; set; }
        public long DiskUsed { get; set; }
        public long DiskTotal { get; set; }
    }
}
