using System.IO;
using Newtonsoft.Json;

namespace GlaDOS_v1_0.Logs
{
	public static class JsonLogger
	{
		private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.json");
		private static readonly object LockObj = new();

		public static void LogException(Exception ex, string? customMessage = null)
		{
			var logEntry = new LogEntry
			{
				Timestamp = DateTime.UtcNow,
				Message = customMessage ?? ex.Message,
				ExceptionType = ex.GetType().FullName,
				StackTrace = ex.StackTrace,
				InnerException = ex.InnerException?.ToString()
			};

			WriteLog(logEntry);
		}

		public static void LogMessage(string message)
		{
			var logEntry = new LogEntry
			{
				Timestamp = DateTime.UtcNow,
				Message = message
			};

			WriteLog(logEntry);
		}

		private static void WriteLog(LogEntry logEntry)
		{
			lock (LockObj)
			{
				var logs = new List<LogEntry>();

				if (File.Exists(LogFilePath))
				{
					try
					{
						var existingContent = File.ReadAllText(LogFilePath);
						logs = JsonConvert.DeserializeObject<List<LogEntry>>(existingContent) ?? new List<LogEntry>();
					}
					catch
					{
						// If deserialization fails, start a new log list.
					}
				}

				logs.Add(logEntry);

				var json = JsonConvert.SerializeObject(logs, Formatting.Indented);
				File.WriteAllText(LogFilePath, json);
			}
		}

		private class LogEntry
		{
			public DateTime Timestamp { get; set; }
			public string? Message { get; set; }
			public string? ExceptionType { get; set; }
			public string? StackTrace { get; set; }
			public string? InnerException { get; set; }
		}
	}
}
