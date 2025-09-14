using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;


namespace SystemMonitor.Common
{
    public class ApiIntegrationPlugin : IMonitorPlugin
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiEndpoint;
        private readonly ILogger<MonitorService> _logger;


        public ApiIntegrationPlugin(IConfiguration configuration, ILogger<MonitorService> logger)
        {
            _httpClient = new HttpClient();
            _apiEndpoint = configuration["ApiIntegration:Endpoint"] ?? throw new ArgumentNullException("API endpoint is not configured.");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ProcessMetricsAsync(SystemMetrics metrics)
        {
            try
            {
                var payload = new
                {
                    cpu = metrics.CpuUsage,
                    ram_used = metrics.RamUsed,
                    disk_used = metrics.DiskUsed,
                };

                var jsonPayload = JsonSerializer.Serialize(payload);

                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                _logger.LogInformation(
                    "Sending metrics to API {Endpoint}, Payload: {jsonPayload}", _apiEndpoint, jsonPayload);

                var response = await _httpClient.PostAsync(_apiEndpoint, content);
                _logger.LogInformation("API response: StatusCode={StatusCode}", response.StatusCode);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending metrics to API {Endpoint}", _apiEndpoint);
            }
        }
    }
}
