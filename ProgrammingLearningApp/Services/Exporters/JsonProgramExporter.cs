using ProgrammingLearningApp.Commands;
using ProgrammingLearningApp.Models;
using System.IO;
using System.Text;

namespace ProgrammingLearningApp.Services
{
	public class JsonProgramExporter : IProgramExporter
	{
		public void Export(Program program, string filePath)
		{
			if (program == null)
				throw new ArgumentNullException(nameof(program));

			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("File path cannot be empty");

			var json = ConvertToJson(program);
			File.WriteAllText(filePath, json);
		}

		private string ConvertToJson(Program program)
		{
			var sb = new StringBuilder();
			sb.AppendLine("{");
			sb.AppendLine($"  \"name\": \"{EscapeJson(program.Name)}\",");
			sb.AppendLine("  \"commands\": [");

			for (int i = 0; i < program.Commands.Count; i++)
			{
				var command = program.Commands[i];
				sb.Append(ConvertCommandToJson(command, 2));
				if (i < program.Commands.Count - 1)
					sb.AppendLine(",");
				else
					sb.AppendLine();
			}

			sb.AppendLine("  ]");
			sb.AppendLine("}");
			return sb.ToString();
		}

		private string ConvertCommandToJson(ICommand command, int indentLevel)
		{
			string indent = new string(' ', indentLevel * 2);
			var sb = new StringBuilder();

			if (command is MoveCommand moveCmd)
			{
				sb.Append(indent);
				sb.Append($"{{ \"type\": \"Move\", \"steps\": {moveCmd.Steps} }}");
			}
			else if (command is TurnCommand turnCmd)
			{
				string direction = turnCmd.TurnDirection == TurnDirection.Left ? "left" : "right";
				sb.Append(indent);
				sb.Append($"{{ \"type\": \"Turn\", \"direction\": \"{direction}\" }}");
			}
			else if (command is RepeatCommand repeatCmd)
			{
				sb.AppendLine(indent + "{");
				sb.AppendLine(indent + "  \"type\": \"Repeat\",");
				sb.AppendLine(indent + $"  \"times\": {repeatCmd.Times},");
				sb.AppendLine(indent + "  \"commands\": [");

				for (int i = 0; i < repeatCmd.Commands.Count; i++)
				{
					var nestedCmd = repeatCmd.Commands[i];
					sb.Append(ConvertCommandToJson(nestedCmd, indentLevel + 2));
					if (i < repeatCmd.Commands.Count - 1)
						sb.AppendLine(",");
					else
						sb.AppendLine();
				}

				sb.AppendLine(indent + "  ]");
				sb.Append(indent + "}");
			}
			else if (command is RepeatUntilCommand repeatUntilCmd)
			{
				string condition = repeatUntilCmd.Condition == Condition.WallAhead
					? "WallAhead" : "GridEdge";

				sb.AppendLine(indent + "{");
				sb.AppendLine(indent + "  \"type\": \"RepeatUntil\",");
				sb.AppendLine(indent + $"  \"condition\": \"{condition}\",");
				sb.AppendLine(indent + "  \"commands\": [");

				for (int i = 0; i < repeatUntilCmd.Commands.Count; i++)
				{
					var nestedCmd = repeatUntilCmd.Commands[i];
					sb.Append(ConvertCommandToJson(nestedCmd, indentLevel + 2));
					if (i < repeatUntilCmd.Commands.Count - 1)
						sb.AppendLine(",");
					else
						sb.AppendLine();
				}

				sb.AppendLine(indent + "  ]");
				sb.Append(indent + "}");
			}

			return sb.ToString();
		}

		private string EscapeJson(string text)
		{
			return text.Replace("\\", "\\\\")
					  .Replace("\"", "\\\"")
					  .Replace("\n", "\\n")
					  .Replace("\r", "\\r")
					  .Replace("\t", "\\t");
		}

		public string GetFileExtension()
		{
			return ".json";
		}

		public string GetFormatName()
		{
			return "JSON Format";
		}
	}
}