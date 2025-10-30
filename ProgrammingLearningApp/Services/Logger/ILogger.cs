using System;

namespace ProgrammingLearningApp.Services.Logger
{
	public interface ILogger
	{
		void Log(string message, LogLevel level = LogLevel.Info);
		void LogInfo(string message);
		void LogWarning(string message);
		void LogError(string message);
		void LogDebug(string message);
	}

	public enum LogLevel
	{
		Debug,
		Info,
		Warning,
		Error
	}
}
