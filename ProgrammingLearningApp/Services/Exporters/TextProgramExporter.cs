using System.IO;

namespace ProgrammingLearningApp.Services
{
	public class TextProgramExporter : IProgramExporter
	{
		public void Export(Program program, string filePath)
		{
			if (program == null)
				throw new ArgumentNullException(nameof(program));

			if (string.IsNullOrWhiteSpace(filePath))
				throw new ArgumentException("File path cannot be empty");

			string content = program.GetTextRepresentation();
			File.WriteAllText(filePath, content);
		}

		public string GetFileExtension()
		{
			return ".txt";
		}

		public string GetFormatName()
		{
			return "Text Format";
		}
	}
}