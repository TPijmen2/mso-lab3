using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProgrammingLearningApp
{
	public class ProgramImporter
	{
		public Program ImportFromFile(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException($"File not found: {filePath}");

			string[] lines = File.ReadAllLines(filePath);
			string programName = Path.GetFileNameWithoutExtension(filePath);

			var commands = ParseCommands(lines, 0, out int _);

			return new Program(programName, commands);
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

				if (trimmedLine.StartsWith("Move "))
				{
					int steps = int.Parse(trimmedLine.Substring(5));
					commands.Add(new MoveCommand(steps));
					i++;
				}
				else if (trimmedLine.StartsWith("Turn "))
				{
					string direction = trimmedLine.Substring(5);
					TurnDirection turnDir = direction.ToLower() == "left"
						? TurnDirection.Left
						: TurnDirection.Right;
					commands.Add(new TurnCommand(turnDir));
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
					i = nextIndex;
				}
				else
				{
					throw new FormatException($"Unknown command: {trimmedLine}");
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