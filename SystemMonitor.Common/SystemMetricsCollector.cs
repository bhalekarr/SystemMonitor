using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SystemMonitor.Common
{
    public class SystemMetricsCollector : ISystemMetricsCollector
    {
        private readonly ILogger<SystemMetricsCollector> _logger;
        public SystemMetricsCollector(ILogger<SystemMetricsCollector> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SystemMetrics> CollectMetricsAsync(CancellationToken cancellationToken)
        {
            return await Task.Run(() => {
                double cpuUsage = GetCpuUsage();
                (long used, long total) ram = GetMemoryUsage();
                (long used, long total) disk = GetDiskUsage();

                return new SystemMetrics
                {
                    CpuUsage = cpuUsage,
                    RamUsed = ram.used,
                    RamTotal = ram.total,
                    DiskUsed = disk.used,
                    DiskTotal = disk.total
                };
            }, cancellationToken);
        }

        private (long used, long total) GetDiskUsage()
        {
            try
            {
                var drive = DriveInfo.GetDrives().FirstOrDefault(d => d.IsReady && d.Name == Path.GetPathRoot(Environment.SystemDirectory));
                if (drive != null)
                {
                    long totalSize = drive.TotalSize / (1024 * 1024); // in MB
                    long freeSpace = drive.TotalFreeSpace / (1024 * 1024); // in MB
                    long usedSpace = totalSize - freeSpace;
                    return (usedSpace, totalSize);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error collecting disk usage");
            }
            return (0, 0);
        }

        private (long used, long total) GetMemoryUsage()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    using var ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                    float available = ramCounter.NextValue();
                    using var totalCounter = new PerformanceCounter("Memory", "Committed Bytes");
                    float committed = totalCounter.NextValue() / (1024.0f * 1024.0f); // Convert to MB
                    long total = (long)(available + committed); // Estimate total as available + committed
                    long used = (long)committed;
                    return (used, total);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error collecting memory usage");
                    return (0, 0);
                }
            }
            else
            {
                // Placeholder for Linux/macOS implementation
                _logger.LogWarning("Memory usage collection not implemented for non-Windows platforms");
                return (0, 0);
            }
        }

        private double GetCpuUsage()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    cpuCounter.NextValue();
                    Thread.Sleep(1000);
                    return Math.Round(cpuCounter.NextValue(), 2);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error collecting CPU usage");
                    return 0.0;
                }
            }
            else
            {
                // Placeholder for Linux/macOS implementation
                _logger.LogWarning("CPU usage collection not implemented for non-Windows platforms");
                return 0.0;
            }
        }
    }
}
