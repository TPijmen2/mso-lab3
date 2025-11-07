using ProgrammingLearningApp.Services;
using ProgrammingLearningApp.Services.Logger;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class LoggingTests : IDisposable
	{
		private readonly string _testLogDirectory;
		private readonly string _testLogFile;

		public LoggingTests()
		{
			// Create a unique test directory for each test run
			_testLogDirectory = Path.Combine(Path.GetTempPath(), $"LoggingTests_{Guid.NewGuid()}");
			Directory.CreateDirectory(_testLogDirectory);
			_testLogFile = Path.Combine(_testLogDirectory, "test.log");
		}

		public void Dispose()
		{
			// Clean up test directory
			if (Directory.Exists(_testLogDirectory))
			{
				try
				{
					Directory.Delete(_testLogDirectory, true);
				}
				catch
				{
					// Ignore cleanup errors
				}
			}
		}

		[Fact]
		public void LoggingService_CanBeInstantiated()
		{
			// Arrange & Act
			var loggingService = new LoggingService();

			// Assert
			Assert.NotNull(loggingService);
		}

		[Fact]
		public void FileLogger_ImplementsILogger()
		{
			// Arrange & Act
			ILogger logger = new FileLogger(_testLogFile);

			// Assert
			Assert.NotNull(logger);
			Assert.IsAssignableFrom<ILogger>(logger);
		}

		[Fact]
		public void FileLogger_CreatesLogDirectory_IfNotExists()
		{
			// Arrange
			string nonExistentPath = Path.Combine(_testLogDirectory, "subdir", "test.log");

			// Act
			var logger = new FileLogger(nonExistentPath);
			logger.LogInfo("Test message");

			// Assert
			Assert.True(Directory.Exists(Path.GetDirectoryName(nonExistentPath)));
			Assert.True(File.Exists(nonExistentPath));
		}

		[Fact]
		public void LogMessage_WritesToFile()
		{
			// Arrange
			var logger = new FileLogger(_testLogFile);
			string testMessage = "Test log message";

			// Act
			logger.LogInfo(testMessage);

			// Assert
			Assert.True(File.Exists(_testLogFile));
			string content = File.ReadAllText(_testLogFile);
			Assert.Contains(testMessage, content);
			Assert.Contains("[INFO", content);
		}

		[Fact]
		public void LogError_WritesToFile()
		{
			// Arrange
			var logger = new FileLogger(_testLogFile);
			string errorMessage = "Test error message";

			// Act
			logger.LogError(errorMessage);

			// Assert
			Assert.True(File.Exists(_testLogFile));
			string content = File.ReadAllText(_testLogFile);
			Assert.Contains(errorMessage, content);
			Assert.Contains("[ERROR", content);
		}

		[Fact]
		public void LogWarning_WritesToFile()
		{
			// Arrange
			var logger = new FileLogger(_testLogFile);
			string warningMessage = "Test warning message";

			// Act
			logger.LogWarning(warningMessage);

			// Assert
			Assert.True(File.Exists(_testLogFile));
			string content = File.ReadAllText(_testLogFile);
			Assert.Contains(warningMessage, content);
			Assert.Contains("[WARNING", content);
		}

		[Fact]
		public void Log_WithNullMessage_HandlesGracefully()
		{
			// Arrange
			var logger = new FileLogger(_testLogFile);

			// Act
			logger.Log(null);

			// Assert
			Assert.True(File.Exists(_testLogFile));
			string content = File.ReadAllText(_testLogFile);
			Assert.Contains("(empty message)", content);
		}

		[Fact]
		public void Log_WithDifferentLogLevels_FormatsCorrectly()
		{
			// Arrange
			var logger = new FileLogger(_testLogFile);

			// Act
			logger.LogDebug("Debug message");
			logger.LogInfo("Info message");
			logger.LogWarning("Warning message");
			logger.LogError("Error message");

			// Assert
			string content = File.ReadAllText(_testLogFile);
			Assert.Contains("[DEBUG", content);
			Assert.Contains("[INFO", content);
			Assert.Contains("[WARNING", content);
			Assert.Contains("[ERROR", content);
			Assert.Contains("Debug message", content);
			Assert.Contains("Info message", content);
			Assert.Contains("Warning message", content);
			Assert.Contains("Error message", content);
		}

		[Fact]
		public void FileLogger_AppendsToExistingFile()
		{
			// Arrange
			var logger = new FileLogger(_testLogFile);
			logger.LogInfo("First message");

			// Act
			logger.LogInfo("Second message");

			// Assert
			string content = File.ReadAllText(_testLogFile);
			Assert.Contains("First message", content);
			Assert.Contains("Second message", content);
		}

		[Fact]
		public void FileLogger_RotatesLogs_WhenSizeExceeded()
		{
			// Arrange
			// Create a logger with very small max size (1 KB)
			var logger = new FileLogger(_testLogFile, maxFileSizeInMB: (long)0.001);
			
			// Write enough data to exceed the size limit
			for (int i = 0; i < 100; i++)
			{
				logger.LogInfo($"Message {i} with some padding to increase size");
			}

			// Act
			System.Threading.Thread.Sleep(100); // Small delay to ensure file operations complete
			var files = Directory.GetFiles(_testLogDirectory, "*.log");

			// Assert
			Assert.True(files.Length > 1, "Expected log rotation to create additional log files");
		}

		[Fact]
		public void LoggingService_UsesSingleton()
		{
			// Arrange & Act
			var instance1 = LoggingService.Instance;
			var instance2 = LoggingService.Instance;

			// Assert
			Assert.Same(instance1, instance2);
		}

		[Fact]
		public void LoggingService_LogsMessages()
		{
			// Arrange
			var logger = new FileLogger(_testLogFile);
			var service = new LoggingService(logger);
			string message = "Service test message";

			// Act
			service.LogInfo(message);

			// Assert
			Assert.True(File.Exists(_testLogFile));
			string content = File.ReadAllText(_testLogFile);
			Assert.Contains(message, content);
		}

		[Fact]
		public void LoggingService_LogsException()
		{
			// Arrange
			var logger = new FileLogger(_testLogFile);
			var service = new LoggingService(logger);
			var exception = new InvalidOperationException("Test exception");

			// Act
			service.LogException(exception, "Test context");

			// Assert
			Assert.True(File.Exists(_testLogFile));
			string content = File.ReadAllText(_testLogFile);
			Assert.Contains("Test context", content);
			Assert.Contains("Test exception", content);
			Assert.Contains("[ERROR", content);
		}
	}
}
