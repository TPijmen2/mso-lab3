using ProgrammingLearningApp.Commands;
using ProgrammingLearningApp.Models;
using System.IO;
using System.Text;

namespace ProgrammingLearningApp.Services
{
	public class HtmlProgramExporter : IProgramExporter
	{
		public void Export(Program program, string filePath)
		{
			if (program == null)
				throw new ArgumentNullException(nameof(program));

			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("File path cannot be empty");

			var html = ConvertToHtml(program);
			File.WriteAllText(filePath, html);
		}

		private string ConvertToHtml(Program program)
		{
			var sb = new StringBuilder();
			sb.AppendLine("<!DOCTYPE html>");
			sb.AppendLine("<html>");
			sb.AppendLine("<head>");
			sb.AppendLine("    <meta charset=\"UTF-8\">");
			sb.AppendLine($"    <title>{EscapeHtml(program.Name)}</title>");
			sb.AppendLine("    <style>");
			sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 20px; }");
			sb.AppendLine("        h1 { color: #333; }");
			sb.AppendLine("        .command { font-weight: bold; color: #2196F3; }");
			sb.AppendLine("        ul { list-style-type: none; }");
			sb.AppendLine("        .indent { margin-left: 30px; }");
			sb.AppendLine("    </style>");
			sb.AppendLine("</head>");
			sb.AppendLine("<body>");
			sb.AppendLine($"    <h1>Program: {EscapeHtml(program.Name)}</h1>");
			sb.AppendLine("    <ul>");

			foreach (var command in program.Commands)
			{
				ConvertCommandToHtml(command, sb, 2);
			}

			sb.AppendLine("    </ul>");
			sb.AppendLine("</body>");
			sb.AppendLine("</html>");
			return sb.ToString();
		}

		private void ConvertCommandToHtml(ICommand command, StringBuilder sb, int indentLevel)
		{
			string indent = new string(' ', indentLevel * 4);

			if (command is MoveCommand moveCmd)
			{
				sb.AppendLine($"{indent}<li><span class=\"command\">Move</span> {moveCmd.Steps}</li>");
			}
			else if (command is TurnCommand turnCmd)
			{
				string direction = turnCmd.TurnDirection == TurnDirection.Left ? "left" : "right";
				sb.AppendLine($"{indent}<li><span class=\"command\">Turn</span> {direction}</li>");
			}
			else if (command is RepeatCommand repeatCmd)
			{
				sb.AppendLine($"{indent}<li><span class=\"command\">Repeat</span> {repeatCmd.Times} times");
				sb.AppendLine($"{indent}    <ul class=\"indent\">");
				foreach (var nestedCmd in repeatCmd.Commands)
				{
					ConvertCommandToHtml(nestedCmd, sb, indentLevel + 2);
				}
				sb.AppendLine($"{indent}    </ul>");
				sb.AppendLine($"{indent}</li>");
			}
			else if (command is RepeatUntilCommand repeatUntilCmd)
			{
				string condition = repeatUntilCmd.Condition == Condition.WallAhead
					? "WallAhead" : "GridEdge";
				sb.AppendLine($"{indent}<li><span class=\"command\">RepeatUntil</span> {condition}");
				sb.AppendLine($"{indent}    <ul class=\"indent\">");
				foreach (var nestedCmd in repeatUntilCmd.Commands)
				{
					ConvertCommandToHtml(nestedCmd, sb, indentLevel + 2);
				}
				sb.AppendLine($"{indent}    </ul>");
				sb.AppendLine($"{indent}</li>");
			}
		}

		private string EscapeHtml(string text)
		{
			return text.Replace("&", "&amp;")
					  .Replace("<", "&lt;")
					  .Replace(">", "&gt;")
					  .Replace("\"", "&quot;")
					  .Replace("'", "&#39;");
		}

		public string GetFileExtension()
		{
			return ".html";
		}

		public string GetFormatName()
		{
			return "HTML Format";
		}
	}
}