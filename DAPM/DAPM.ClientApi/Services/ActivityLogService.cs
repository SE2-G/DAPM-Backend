////using DAPM.ClientApi.Services.Interfaces;
////using Microsoft.Extensions.Logging;
////using System;
////using System.IO;
////using System.Threading.Tasks;

////namespace DAPM.ClientApi.Services
////{
////    public class ActivityLogService : IActivityLogService
////    {
////        private readonly ILogger<ActivityLogService> _logger;
////        private readonly string _logFilePath;

////        public ActivityLogService(ILogger<ActivityLogService> logger)
////        {
////            _logger = logger;
////            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "ActivityLog.txt");

////            var logDirectory = Path.GetDirectoryName(_logFilePath);
////            if (!Directory.Exists(logDirectory))
////            {
////                Directory.CreateDirectory(logDirectory);
////            }
////        }

////        public async Task LogUserActivity(string userName, string action, string result, DateTime timestamp)
////        {
////            try
////            {
////                var logEntry = $"{timestamp:yyyy-MM-dd HH:mm:ss} | User: {userName} | Action: {action} | Result: {result}";
////                await File.AppendAllTextAsync(_logFilePath, logEntry + Environment.NewLine);
////                _logger.LogInformation($"Logged activity: {logEntry}");
////            }
////            catch (Exception ex)
////            {
////                _logger.LogError($"Error logging activity: {ex.Message}");
////            }
////        }

////      public async Task<Stream> DownloadActivityLogAsync()
////{
////    try
////    {
////        if (!File.Exists(_logFilePath))
////        {
////            throw new FileNotFoundException("Activity log file not found.");
////        }

////        // Prepare the memory stream
////        var memoryStream = new MemoryStream();
////        await using (var fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read))
////        {
////            await fileStream.CopyToAsync(memoryStream);
////        }

////        memoryStream.Position = 0; 
////        _logger.LogInformation("Activity log file successfully prepared for download.");
////        return memoryStream;
////    }
////    catch (Exception ex)
////    {
////        _logger.LogError($"Error preparing activity log for download: {ex.Message}");
////        throw;
////    }
////}

////    }
////}
//using DAPM.ClientApi.Services.Interfaces;
//using Microsoft.Extensions.Logging;
//using System;
//using System.IO;
//using System.Threading.Tasks;

//namespace DAPM.ClientApi.Services
//{
//    public class ActivityLogService : IActivityLogService
//    {
//        private readonly ILogger<ActivityLogService> _logger;
//        private readonly string _logFilePath;

//        // Lock to prevent concurrent file access
//        private static readonly object _lock = new object();

//        public ActivityLogService(ILogger<ActivityLogService> logger)
//        {
//            _logger = logger;
//            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "ActivityLog.txt");

//            var logDirectory = Path.GetDirectoryName(_logFilePath);
//            if (!Directory.Exists(logDirectory))
//            {
//                Directory.CreateDirectory(logDirectory);
//            }
//        }

//        public async Task LogUserActivity(string userName, string action, string result, DateTime timestamp)
//        {
//            try
//            {
//                // Format the log entry correctly
//                var logEntry = $"{timestamp:yyyy-MM-dd HH:mm:ss} | User: {userName} | Action: {action} | Result: {result}{Environment.NewLine}";

//                // Lock to prevent concurrent writes
//                lock (_lock)
//                {
//                    File.AppendAllText(_logFilePath, logEntry);
//                }

//                // Log to the console
//                _logger.LogInformation($"Logged activity: {logEntry.Trim()}");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error logging activity: {ex.Message}");
//            }
//        }

//        public async Task<Stream> DownloadActivityLogAsync()
//        {
//            try
//            {
//                if (!File.Exists(_logFilePath))
//                {
//                    throw new FileNotFoundException("Activity log file not found.");
//                }

//                // Prepare the memory stream
//                var memoryStream = new MemoryStream();
//                await using (var fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read))
//                {
//                    await fileStream.CopyToAsync(memoryStream);
//                }

//                memoryStream.Position = 0;
//                _logger.LogInformation("Activity log file successfully prepared for download.");
//                return memoryStream;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error preparing activity log for download: {ex.Message}");
//                throw;
//            }
//        }
//    }
//}
//using DAPM.ClientApi.Services.Interfaces;
//using Microsoft.Extensions.Logging;
//using System;
//using System.IO;
//using System.Threading.Tasks;

//namespace DAPM.ClientApi.Services
//{
//    public class ActivityLogService : IActivityLogService
//    {
//        private readonly ILogger<ActivityLogService> _logger;
//        private readonly string _logFilePath;

//        // Lock to prevent concurrent file access
//        private static readonly object _lock = new object();

//        /// <summary>
//        /// Initializes a new instance of the <see cref="ActivityLogService"/> class.
//        /// </summary>
//        /// <param name="logger">The logger instance for logging information and errors.</param>
//        public ActivityLogService(ILogger<ActivityLogService> logger)
//        {
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "ActivityLog.txt");

//            var logDirectory = Path.GetDirectoryName(_logFilePath);
//            if (!Directory.Exists(logDirectory))
//            {
//                Directory.CreateDirectory(logDirectory);
//                _logger.LogInformation($"Created log directory at: {logDirectory}");
//            }
//        }

//        /// <summary>
//        /// Logs user activity with additional IP address information.
//        /// </summary>
//        /// <param name="userName">The name of the user performing the action.</param>
//        /// <param name="action">A description of the action performed.</param>
//        /// <param name="result">The outcome of the action.</param>
//        /// <param name="timestamp">The UTC timestamp when the action occurred.</param>
//        /// <param name="clientIp">The IP address of the client making the request.</param>
//        /// <param name="serverIp">The IP address of the server handling the request.</param>
//        /// <returns>A task representing the asynchronous operation.</returns>
//        public async Task LogUserActivity(string userName, string action, string result, DateTime timestamp, string clientIp)
//        {
//            try
//            {
//                // Format the log entry to include IP addresses
//                var logEntry = $"{timestamp:yyyy-MM-dd HH:mm:ss} | User: {userName} | Action: {action} | Result: {result} | Client IP: {clientIp} {Environment.NewLine}";

//                // Lock to prevent concurrent writes
//                lock (_lock)
//                {
//                    File.AppendAllText(_logFilePath, logEntry);
//                }

//                // Log to the console for immediate visibility
//                _logger.LogInformation($"Logged activity: {logEntry.Trim()}");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error logging activity: {ex.Message}");
//            }

//            await Task.CompletedTask; // Since File.AppendAllText is synchronous, return completed task
//        }

//        /// <summary>
//        /// Downloads the activity log as a stream.
//        /// </summary>
//        /// <returns>A stream containing the activity log data.</returns>
//        public async Task<Stream> DownloadActivityLogAsync()
//        {
//            try
//            {
//                if (!File.Exists(_logFilePath))
//                {
//                    throw new FileNotFoundException("Activity log file not found.");
//                }

//                // Prepare the memory stream
//                var memoryStream = new MemoryStream();
//                await using (var fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
//                {
//                    await fileStream.CopyToAsync(memoryStream);
//                }

//                memoryStream.Position = 0;
//                _logger.LogInformation("Activity log file successfully prepared for download.");
//                return memoryStream;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError($"Error preparing activity log for download: {ex.Message}");
//                throw;
//            }
//        }
//    }
//}
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

        // Lock to prevent concurrent file access
        private static readonly object _lock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="ActivityLogService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance for logging information and errors.</param>
        public ActivityLogService(ILogger<ActivityLogService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", "ActivityLog.txt");

            var logDirectory = Path.GetDirectoryName(_logFilePath);
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
                _logger.LogInformation($"Created log directory at: {logDirectory}");
            }
        }

        /// <summary>
        /// Logs user activity with the client's IP address.
        /// </summary>
        /// <param name="userName">The name of the user performing the action.</param>
        /// <param name="action">A description of the action performed.</param>
        /// <param name="result">The outcome of the action.</param>
        /// <param name="timestamp">The UTC timestamp when the action occurred.</param>
        /// <param name="clientIp">The IP address of the client making the request.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LogUserActivity(string userName, string action, string result, DateTime timestamp, string clientIp)
        {
            try
            {
                // Format the log entry to include only Client IP
                var logEntry = $"{timestamp:yyyy-MM-dd HH:mm:ss} | User: {userName} | Action: {action} | Result: {result} | Client IP: {clientIp}{Environment.NewLine}";

                // Lock to prevent concurrent writes
                lock (_lock)
                {
                    File.AppendAllText(_logFilePath, logEntry);
                }

                // Log to the console for immediate visibility
                _logger.LogInformation($"Logged activity: {logEntry.Trim()}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error logging activity: {ex.Message}");
            }

            await Task.CompletedTask; // Since File.AppendAllText is synchronous, return completed task
        }

        /// <summary>
        /// Downloads the activity log as a stream.
        /// </summary>
        /// <returns>A stream containing the activity log data.</returns>
        public async Task<Stream> DownloadActivityLogAsync()
        {
            try
            {
                if (!File.Exists(_logFilePath))
                {
                    throw new FileNotFoundException("Activity log file not found.");
                }

                // Prepare the memory stream
                var memoryStream = new MemoryStream();
                await using (var fileStream = new FileStream(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await fileStream.CopyToAsync(memoryStream);
                }

                memoryStream.Position = 0;
                _logger.LogInformation("Activity log file successfully prepared for download.");
                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error preparing activity log for download: {ex.Message}");
                throw;
            }
        }
    }
}
