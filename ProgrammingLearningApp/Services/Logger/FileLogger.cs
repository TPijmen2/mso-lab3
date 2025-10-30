using System;
using System.IO;
using System.Text;

namespace ProgrammingLearningApp.Services.Logger
{
	public class FileLogger : ILogger
	{
		private readonly string _logFilePath;
		private readonly long _maxFileSizeBytes;
		private readonly object _lockObject = new object();

		public FileLogger(string logFilePath = null, long maxFileSizeInMB = 10)
		{
			// Default log file path in application directory
			_logFilePath = logFilePath ?? Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				"ProgrammingLearningApp",
				"Logs",
				$"app_{DateTime.Now:yyyyMMdd}.log"
			);

			_maxFileSizeBytes = maxFileSizeInMB * 1024 * 1024;

			// Ensure log directory exists
			var directory = Path.GetDirectoryName(_logFilePath);
			if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
		}

		public void Log(string message, LogLevel level = LogLevel.Info)
		{
			if (string.IsNullOrEmpty(message))
			{
				message = "(empty message)";
			}

			lock (_lockObject)
			{
				try
				{
					// Check if log rotation is needed
					RotateLogIfNeeded();

					// Format log entry
					string logEntry = FormatLogEntry(message, level);

					// Append to file
					File.AppendAllText(_logFilePath, logEntry + Environment.NewLine, Encoding.UTF8);
				}
				catch (Exception ex)
				{
					// If logging fails, write to console as fallback
					Console.WriteLine($"[LOGGING ERROR] {ex.Message}");
					Console.WriteLine($"[{level}] {message}");
				}
			}
		}

		public void LogInfo(string message)
		{
			Log(message, LogLevel.Info);
		}

		public void LogWarning(string message)
		{
			Log(message, LogLevel.Warning);
		}

		public void LogError(string message)
		{
			Log(message, LogLevel.Error);
		}

		public void LogDebug(string message)
		{
			Log(message, LogLevel.Debug);
		}

		private string FormatLogEntry(string message, LogLevel level)
		{
			string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
			string levelStr = level.ToString().ToUpper().PadRight(7);
			return $"[{timestamp}] [{levelStr}] {message}";
		}

		private void RotateLogIfNeeded()
		{
			if (!File.Exists(_logFilePath))
			{
				return;
			}

			var fileInfo = new FileInfo(_logFilePath);
			if (fileInfo.Length >= _maxFileSizeBytes)
			{
				// Create archive file name with unique suffix
				string directory = Path.GetDirectoryName(_logFilePath);
				string fileNameWithoutExt = Path.GetFileNameWithoutExtension(_logFilePath);
				string extension = Path.GetExtension(_logFilePath);
				
				// Find a unique filename
				string archivePath;
				int counter = 1;
				do
				{
					string timestamp = DateTime.Now.ToString("HHmmss");
					string suffix = counter == 1 ? timestamp : $"{timestamp}_{counter}";
					string archiveFileName = $"{fileNameWithoutExt}_{suffix}{extension}";
					archivePath = Path.Combine(directory, archiveFileName);
					counter++;
				} while (File.Exists(archivePath) && counter < 1000);

				// Move current log to archive
				if (!File.Exists(archivePath))
				{
					File.Move(_logFilePath, archivePath);
				}
				else
				{
					// If we still can't find a unique name, just delete the old file
					File.Delete(_logFilePath);
				}
			}
		}

		public string GetLogFilePath()
		{
			return _logFilePath;
		}
	}
}
