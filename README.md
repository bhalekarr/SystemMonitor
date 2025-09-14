System Monitor Application


Overview

This is a cross-platform console application built with .NET 8.0 that monitors system resources (CPU, memory, and disk usage) in real-time. It supports a plugin architecture for extensibility, allowing custom integrations such as logging to a file or posting metrics to a configurable REST API endpoint. The application uses dependency injection and follows a clean architecture pattern, with all components consolidated into a single project for simplicity.



Prerequisites

.NET 8.0 SDK: Ensure the .NET 8.0 SDK is installed. 
Verify with:dotnet --version


Operating System: Tested on Windows; Linux/macOS support is partially implemented with placeholders for platform-specific metric collection.
Text Editor/IDE: Visual Studio, Visual Studio Code, or any text editor for editing appsettings.json.

How to Build and Run

Clone or Download the Project:
Clone the repository or download the project files to your local machine from https://github.com/bhalekarr/SystemMonitor.git.


Restore Dependencies:
Navigate to the project directory (SystemMonitor.Common) and restore NuGet packages:dotnet restore


Build the Project:
Build the solution:dotnet build
Navigate Build directory in "\SystemMonitor\SystemMonitor\bin\Release\net8.0" with appsetting.json file.
Single click deployement or setup file with Application files folder and SystemMonitor.manifest file can generate in above folder or user specific folder. 


Configure appsettings.json:
Update appsettings.json in the project root to set the monitoring interval and API endpoint:{
  "Monitoring": {
    "IntervalSeconds": 10
  },
  "ApiIntegration": {
    "Endpoint": "https://your-api-endpoint.com/metrics"
  }
}



Run the Application:
Execute the application: F5
Execute the application: dotnet run --project SystemMonitor


The application will display system metrics (CPU, RAM, disk usage) in the console every 10 seconds (configurable) and invoke plugins (e.g., file logging, API posting).

Log File:
Log file created in Current Directory/logs if not existed, filename: system-monitor-20250914.log (logs/system-monitor-.log) by daily rolloverinterval.
Sample logfile is attached with email. filename: system-monitor-20250914.log


Design Decisions and Challenges:

The application uses a clean architecture with dependency injection (via Microsoft.Extensions.DependencyInjection) to ensure modularity and testability. All components, including the monitoring service, metrics collector, and plugins, are consolidated into the SystemMonitor.Common project to simplify the structure while maintaining extensibility. The plugin system is implemented via the IMonitorPlugin interface, with sample plugins for file logging and API integration (ApiIntegrationPlugin). 

Challenges: 
Mostly output of the json file while creating exe.
Change Performance Counter to GlobalMemoryStatusEx from kernel32.dll library. Used ullTotalPhys (total physical memory) and ullAvailPhys (available physical memory) to calculate accurate memory metrics.

Cross-Platform Metric Collection: Windows-specific metrics use PerformanceCounter, with placeholders for Linux/macOS. Implementing Linux/macOS support (e.g., via /proc/stat for CPU) requires additional platform-specific code.
Dependency Management: Consolidating projects required careful handling of namespaces and dependencies to avoid conflicts.
Error Handling: Robust exception handling was added to manage issues like unavailable drives or API failures.

Limitations and Corner Cases

Platform Support: Full monitoring is implemented for Windows only due to reliance on PerformanceCounter. Linux/macOS support requires additional implementations for CPU and memory metrics.
API Endpoint: The ApiIntegrationPlugin requires a valid endpoint in appsettings.json. If the endpoint is invalid or unreachable, errors are logged to the console.
Performance: Frequent metric collection (every 10 seconds by default) may impact performance on resource-constrained systems.
File Logging: The log writes to "logs/system-monitor-.log" in the output directory, which may grow large over time.

Project Structure

SystemMonitor.Common:
Program.cs: Entry point, sets up dependency injection and configuration.
IMonitorService.cs, MonitorService.cs: Core monitoring logic, orchestrates metric collection and plugin execution.
ISystemMetricsCollector.cs, SystemMetricsCollector.cs: Collects CPU, RAM, and disk usage metrics.
SystemMetrics.cs: Data model for metrics.
IMonitorPlugin.cs, ApiIntegrationPlugin.cs: Plugin interface and implementations for logging and API integration.
IPluginManager.cs, PluginManager.cs: Manages plugin execution.
appsettings.json: Configuration for monitoring interval and API endpoint.



Notes

Logs are written to "logs/system-monitor-.log" in the output directory.
Update the API endpoint in appsettings.json to a valid URL for testing ApiIntegrationPlugin.
For cross-platform support, implement Linux/macOS-specific metric collection in SystemMetricsCollector.cs.
The application is designed to be extensible; new plugins can be added by implementing IMonitorPlugin.
