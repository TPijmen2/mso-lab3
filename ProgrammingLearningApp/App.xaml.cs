using ProgrammingLearningApp.Services;
using System;

namespace ProgrammingLearningApp
{
	public partial class App : System.Windows.Application
	{
		protected override void OnStartup(System.Windows.StartupEventArgs e)
		{
			base.OnStartup(e);
			
			LoggingService.Instance.LogInfo("=== Application Starting ===");
			LoggingService.Instance.LogInfo($"Application version: 2.0");
			LoggingService.Instance.LogInfo($"Start time: {DateTime.Now}");
			
			// Handle unhandled exceptions
			AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
			DispatcherUnhandledException += OnDispatcherUnhandledException;
		}

		protected override void OnExit(System.Windows.ExitEventArgs e)
		{
			LoggingService.Instance.LogInfo("=== Application Shutting Down ===");
			LoggingService.Instance.LogInfo($"Exit code: {e.ApplicationExitCode}");
			LoggingService.Instance.LogInfo($"End time: {DateTime.Now}");
			
			base.OnExit(e);
		}

		private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var exception = e.ExceptionObject as Exception;
			LoggingService.Instance.LogException(exception, "Unhandled application exception");
		}

		private void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			LoggingService.Instance.LogException(e.Exception, "Unhandled dispatcher exception");
			e.Handled = false; // Let the application handle it normally
		}
	}
}