using ProgrammingLearningApp;
using ProgrammingLearningApp.Commands;
using ProgrammingLearningApp.Models;
using ProgrammingLearningApp.Services;
using System.IO;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class ExporterTests
	{
		private readonly string tempDirectory;

		public ExporterTests()
		{
			tempDirectory = Path.Combine(Path.GetTempPath(), "ExporterTests");
			Directory.CreateDirectory(tempDirectory);
		}

		[Fact]
		public void TextExporter_Export_CreatesFile()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(5),
				new TurnCommand(TurnDirection.Right)
			};
			var program = new Program("Test", commands);
			var exporter = new TextProgramExporter();
			string filePath = Path.Combine(tempDirectory, "test.txt");

			// Act
			exporter.Export(program, filePath);

			// Assert
			Assert.True(File.Exists(filePath));
			string content = File.ReadAllText(filePath);
			Assert.Contains("Move 5", content);
			Assert.Contains("Turn right", content);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void TextExporter_Export_NullProgram_ThrowsException()
		{
			// Arrange
			var exporter = new TextProgramExporter();
			string filePath = Path.Combine(tempDirectory, "test.txt");

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => exporter.Export(null, filePath));
		}

		[Fact]
		public void TextExporter_Export_EmptyFilePath_ThrowsException()
		{
			// Arrange
			var program = new Program("Test", new List<ICommand>());
			var exporter = new TextProgramExporter();

			// Act & Assert
			Assert.Throws<ArgumentException>(() => exporter.Export(program, ""));
		}

		[Fact]
		public void TextExporter_GetFileExtension_ReturnsTxt()
		{
			// Arrange
			var exporter = new TextProgramExporter();

			// Act
			string extension = exporter.GetFileExtension();

			// Assert
			Assert.Equal(".txt", extension);
		}

		[Fact]
		public void TextExporter_GetFormatName_ReturnsCorrectName()
		{
			// Arrange
			var exporter = new TextProgramExporter();

			// Act
			string formatName = exporter.GetFormatName();

			// Assert
			Assert.Equal("Text Format", formatName);
		}

		[Fact]
		public void JsonExporter_Export_CreatesValidJsonFile()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(10),
				new TurnCommand(TurnDirection.Left)
			};
			var program = new Program("JsonTest", commands);
			var exporter = new JsonProgramExporter();
			string filePath = Path.Combine(tempDirectory, "test.json");

			// Act
			exporter.Export(program, filePath);

			// Assert
			Assert.True(File.Exists(filePath));
			string content = File.ReadAllText(filePath);
			Assert.Contains("\"name\": \"JsonTest\"", content);
			Assert.Contains("\"type\": \"Move\"", content);
			Assert.Contains("\"steps\": 10", content);
			Assert.Contains("\"type\": \"Turn\"", content);
			Assert.Contains("\"direction\": \"left\"", content);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void JsonExporter_Export_WithRepeatCommand_CreatesValidJson()
		{
			// Arrange
			var innerCommands = new List<ICommand> { new MoveCommand(1) };
			var commands = new List<ICommand>
			{
				new RepeatCommand(3, innerCommands)
			};
			var program = new Program("RepeatTest", commands);
			var exporter = new JsonProgramExporter();
			string filePath = Path.Combine(tempDirectory, "repeat.json");

			// Act
			exporter.Export(program, filePath);

			// Assert
			Assert.True(File.Exists(filePath));
			string content = File.ReadAllText(filePath);
			Assert.Contains("\"type\": \"Repeat\"", content);
			Assert.Contains("\"times\": 3", content);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void JsonExporter_GetFileExtension_ReturnsJson()
		{
			// Arrange
			var exporter = new JsonProgramExporter();

			// Act
			string extension = exporter.GetFileExtension();

			// Assert
			Assert.Equal(".json", extension);
		}

		[Fact]
		public void HtmlExporter_Export_CreatesValidHtmlFile()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(5),
				new TurnCommand(TurnDirection.Right)
			};
			var program = new Program("HtmlTest", commands);
			var exporter = new HtmlProgramExporter();
			string filePath = Path.Combine(tempDirectory, "test.html");

			// Act
			exporter.Export(program, filePath);

			// Assert
			Assert.True(File.Exists(filePath));
			string content = File.ReadAllText(filePath);
			Assert.Contains("<!DOCTYPE html>", content);
			Assert.Contains("<html>", content);
			Assert.Contains("HtmlTest", content);
			Assert.Contains("Move", content);
			Assert.Contains("Turn", content);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void HtmlExporter_Export_EscapesSpecialCharacters()
		{
			// Arrange
			var commands = new List<ICommand> { new MoveCommand(1) };
			var program = new Program("Test<>&\"'", commands);
			var exporter = new HtmlProgramExporter();
			string filePath = Path.Combine(tempDirectory, "escape.html");

			// Act
			exporter.Export(program, filePath);

			// Assert
			Assert.True(File.Exists(filePath));
			string content = File.ReadAllText(filePath);
			Assert.Contains("&lt;", content);
			Assert.Contains("&gt;", content);
			Assert.Contains("&amp;", content);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void HtmlExporter_Export_WithNestedRepeat_FormatsCorrectly()
		{
			// Arrange
			var innerCommands = new List<ICommand> { new MoveCommand(2) };
			var commands = new List<ICommand>
			{
				new RepeatCommand(4, innerCommands)
			};
			var program = new Program("NestedTest", commands);
			var exporter = new HtmlProgramExporter();
			string filePath = Path.Combine(tempDirectory, "nested.html");

			// Act
			exporter.Export(program, filePath);

			// Assert
			Assert.True(File.Exists(filePath));
			string content = File.ReadAllText(filePath);
			Assert.Contains("Repeat", content);
			Assert.Contains("4 times", content);
			Assert.Contains("class=\"indent\"", content);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void HtmlExporter_GetFileExtension_ReturnsHtml()
		{
			// Arrange
			var exporter = new HtmlProgramExporter();

			// Act
			string extension = exporter.GetFileExtension();

			// Assert
			Assert.Equal(".html", extension);
		}

		[Fact]
		public void HtmlExporter_GetFormatName_ReturnsCorrectName()
		{
			// Arrange
			var exporter = new HtmlProgramExporter();

			// Act
			string formatName = exporter.GetFormatName();

			// Assert
			Assert.Equal("HTML Format", formatName);
		}
	}
}
