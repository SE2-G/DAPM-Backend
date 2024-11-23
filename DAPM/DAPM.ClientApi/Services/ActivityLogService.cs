using DAPM.ClientApi.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DAPM.ClientApi.Services
{
    public class ActivityLogService : IActivityLogService
    {
        private readonly ILogger<ActivityLogService> _logger;
        private readonly string _logFilePath;

        public ActivityLogService(ILogger<ActivityLogService> logger)
        {
            _logger = logger;
            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "ActivityLog.txt");

            var logDirectory = Path.GetDirectoryName(_logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
        }

        public async Task LogUserActivity(string userName, string action, string result, DateTime timestamp)
        {
            try
            {
                var logEntry = $"{timestamp:yyyy-MM-dd HH:mm:ss} | User: {userName} | Action: {action} | Result: {result}";
                await File.AppendAllTextAsync(_logFilePath, logEntry + Environment.NewLine);
                _logger.LogInformation($"Logged activity: {logEntry}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging activity: {ex.Message}");
            }
        }

    }
}
