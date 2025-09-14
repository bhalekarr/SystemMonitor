using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Reflection;
using SystemMonitor.Common;

namespace SystemMonitor
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() // Keep console logging
                .WriteTo.File(
                    path: "logs/system-monitor-.log", // Log file path with date suffix
                    rollingInterval: RollingInterval.Day, // New file each day
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            try
            {
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

                var services = new ServiceCollection();
                ConfigureServices(services, configuration);

                using var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var monitorService = scope.ServiceProvider.GetService<IMonitorService>();
                if (monitorService == null)
                {
                    Log.Error("Monitor Service is not available.");
                    return;
                }

                var cts = new CancellationTokenSource();
                Console.CancelKeyPress += (s, e) =>
                {
                    Log.Information("Cancellation requested via Ctrl+C");
                    cts.Cancel();
                    e.Cancel = true;
                };
                Log.Information("Monitor Service started......");
                await monitorService.StartMonitoringAsync(cts.Token);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred in the application");
            }
            finally
            {
                Log.Information("Monitor Service End......");
                Log.CloseAndFlush();
            }

        }

        private static void ConfigureServices(ServiceCollection services, IConfiguration configuration)
        {
            services.AddLogging(logging => {
                logging.ClearProviders();
                logging.AddSerilog();
            });

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<ISystemMetricsCollector, SystemMetricsCollector>();
            services.AddSingleton<IMonitorService, MonitorService>();
            services.AddSingleton<IPluginManager, PluginManager>();
            services.AddSingleton<IMonitorPlugin, ApiIntegrationPlugin>();
        }
    }
}