using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProgrammingLearningApp.Services;

namespace ProgrammingLearningApp
{
	public class ProgramImporter
	{
		public Program ImportFromFile(string filePath)
		{
			LoggingService.Instance.LogInfo($"Importing program from file: {filePath}");
			
			if (!File.Exists(filePath))
			{
				LoggingService.Instance.LogError($"File not found: {filePath}");
				throw new FileNotFoundException($"File not found: {filePath}");
			}

			try
			{
				string[] lines = File.ReadAllLines(filePath);
				string programName = Path.GetFileNameWithoutExtension(filePath);

				LoggingService.Instance.LogDebug($"Parsing {lines.Length} lines from file");
				var commands = ParseCommands(lines, 0, out int _);

				LoggingService.Instance.LogInfo($"Successfully imported program '{programName}' with {commands.Count} top-level commands");
				return new Program(programName, commands);
			}
			catch (FormatException ex)
			{
				LoggingService.Instance.LogError($"Format error while parsing program: {ex.Message}");
				throw;
			}
			catch (Exception ex)
			{
				LoggingService.Instance.LogException(ex, $"Error importing program from {filePath}");
				throw;
			}
		}

		private List<ICommand> ParseCommands(string[] lines, int startIndex, out int endIndex)
		{
			var commands = new List<ICommand>();
			int i = startIndex;

			while (i < lines.Length)
			{
				string line = lines[i];
				int indentLevel = GetIndentLevel(line);
				string trimmedLine = line.Trim();

				// Skip empty lines
				if (string.IsNullOrWhiteSpace(trimmedLine))
				{
					i++;
					continue;
				}

				// Check if this line belongs to the current level
				if (startIndex > 0 && indentLevel < GetIndentLevel(lines[startIndex - 1]))
				{
					endIndex = i;
					return commands;
				}

				try
				{
					if (trimmedLine.StartsWith("Move "))
					{
						int steps = int.Parse(trimmedLine.Substring(5));
						commands.Add(new MoveCommand(steps));
						LoggingService.Instance.LogDebug($"Parsed command: Move {steps}");
						i++;
					}
					else if (trimmedLine.StartsWith("Turn "))
					{
						string direction = trimmedLine.Substring(5);
						TurnDirection turnDir = direction.ToLower() == "left"
							? TurnDirection.Left
							: TurnDirection.Right;
						commands.Add(new TurnCommand(turnDir));
						LoggingService.Instance.LogDebug($"Parsed command: Turn {direction}");
						i++;
					}
					else if (trimmedLine.StartsWith("Repeat "))
					{
						// Parse "Repeat X times"
						string[] parts = trimmedLine.Split(' ');
						int times = int.Parse(parts[1]);

						// Parse the commands inside the repeat block
						i++; // Move to first command inside repeat
						var repeatCommands = ParseCommands(lines, i, out int nextIndex);
						commands.Add(new RepeatCommand(times, repeatCommands));
						LoggingService.Instance.LogDebug($"Parsed command: Repeat {times} times with {repeatCommands.Count} sub-commands");
						i = nextIndex;
					}
					else
					{
						LoggingService.Instance.LogError($"Unknown command at line {i + 1}: {trimmedLine}");
						throw new FormatException($"Unknown command: {trimmedLine}");
					}
				}
				catch (FormatException)
				{
					throw;
				}
				catch (Exception ex)
				{
					LoggingService.Instance.LogError($"Error parsing line {i + 1}: {trimmedLine} - {ex.Message}");
					throw new FormatException($"Error parsing command at line {i + 1}: {trimmedLine}", ex);
				}
			}

			endIndex = i;
			return commands;
		}

		private int GetIndentLevel(string line)
		{
			int spaces = 0;
			foreach (char c in line)
			{
				if (c == ' ')
					spaces++;
				else
					break;
			}
			return spaces / 4; // 4 spaces per indent level
		}
	}
}