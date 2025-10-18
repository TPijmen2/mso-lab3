using System;
using System.IO;

namespace ProgrammingLearningApp
{
	public class CommandLineInterface
	{
		private Program currentProgram;

		public void Run()
		{
			Console.WriteLine("=== Programming Learning App ===");
			Console.WriteLine();

			// Step 1: Load a program
			LoadProgram();

			if (currentProgram == null)
			{
				Console.WriteLine("No program loaded. Exiting.");
				return;
			}

			Console.WriteLine($"\nLoaded program: {currentProgram.Name}");
			Console.WriteLine("\nProgram content:");
			Console.WriteLine(currentProgram.GetTextRepresentation());
			Console.WriteLine();

			// Step 2: Choose action
			ChooseAction();
		}

		private void LoadProgram()
		{
			Console.WriteLine("Step 1: Select a program");
			Console.WriteLine("1. Basic example program");
			Console.WriteLine("2. Advanced example program");
			Console.WriteLine("3. Expert example program");
			Console.WriteLine("4. Import from file");
			Console.Write("\nYour choice: ");

			string choice = Console.ReadLine();

			switch (choice)
			{
				case "1":
					currentProgram = ProgramFactory.CreateBasicProgram();
					break;
				case "2":
					currentProgram = ProgramFactory.CreateAdvancedProgram();
					break;
				case "3":
					currentProgram = ProgramFactory.CreateExpertProgram();
					break;
				case "4":
					ImportProgramFromFile();
					break;
				default:
					Console.WriteLine("Invalid choice.");
					break;
			}
		}

		private void ImportProgramFromFile()
		{
			Console.Write("Enter file path: ");
			string filePath = Console.ReadLine();

			try
			{
				var importer = new ProgramImporter();
				currentProgram = importer.ImportFromFile(filePath);
				Console.WriteLine("Program imported successfully!");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error importing program: {ex.Message}");
			}
		}

		private void ChooseAction()
		{
			Console.WriteLine("Step 2: Choose an action");
			Console.WriteLine("1. Execute program");
			Console.WriteLine("2. Calculate metrics");
			Console.Write("\nYour choice: ");

			string choice = Console.ReadLine();

			switch (choice)
			{
				case "1":
					ExecuteProgram();
					break;
				case "2":
					CalculateMetrics();
					break;
				default:
					Console.WriteLine("Invalid choice.");
					break;
			}
		}

		private void ExecuteProgram()
		{
			Console.WriteLine("\n=== Executing Program ===");
			var result = currentProgram.Execute();
			Console.WriteLine(result.ToString());
		}

		private void CalculateMetrics()
		{
			Console.WriteLine("\n=== Program Metrics ===");
			var metrics = currentProgram.CalculateMetrics();
			Console.WriteLine(metrics.ToString());
		}
	}
}