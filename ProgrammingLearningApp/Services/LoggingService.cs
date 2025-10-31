using ProgrammingLearningApp.Services.Logger;
using System;

namespace ProgrammingLearningApp.Services
{
	public class LoggingService
	{
		private static LoggingService _instance;
		private static readonly object _lock = new object();
		private readonly ILogger _logger;

		public LoggingService() : this(new FileLogger())
		{
		}

		public LoggingService(ILogger logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public static LoggingService Instance
		{
			get
			{
				if (_instance == null)
				{
					lock (_lock)
					{
						if (_instance == null)
						{
							_instance = new LoggingService();
						}
					}
				}
				return _instance;
			}
		}

		public void Log(string message, LogLevel level = LogLevel.Info)
		{
			_logger.Log(message, level);
		}

		public void LogInfo(string message)
		{
			_logger.LogInfo(message);
		}

		public void LogWarning(string message)
		{
			_logger.LogWarning(message);
		}

		public void LogError(string message)
		{
			_logger.LogError(message);
		}

		public void LogDebug(string message)
		{
			_logger.LogDebug(message);
		}

		public void LogException(Exception ex, string context = null)
		{
			string message = string.IsNullOrEmpty(context)
				? $"Exception: {ex.Message}\nStackTrace: {ex.StackTrace}"
				: $"{context}\nException: {ex.Message}\nStackTrace: {ex.StackTrace}";

			_logger.LogError(message);
		}

		// Allow access to underlying logger for testing
		public ILogger GetLogger()
		{
			return _logger;
		}
	}
}
