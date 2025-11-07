using ProgrammingLearningApp.Services;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class GridFileParserTests
	{
		private readonly string tempDirectory;

		public GridFileParserTests()
		{
			tempDirectory = Path.Combine(Path.GetTempPath(), "GridFileParserTests");
			Directory.CreateDirectory(tempDirectory);
		}

		[Fact]
		public void ParseFromFile_ValidGrid_ReturnsExercise()
		{
			// Arrange
			string content = "ooo\nooo\noox";
			string filePath = Path.Combine(tempDirectory, "test_grid.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act
			var exercise = parser.ParseFromFile(filePath);

			// Assert
			Assert.NotNull(exercise);
			Assert.Equal("test_grid", exercise.Name);
			Assert.Equal(3, exercise.Grid.Width);
			Assert.Equal(3, exercise.Grid.Height);
			Assert.Equal(2, exercise.Grid.EndPosition.X);
			Assert.Equal(0, exercise.Grid.EndPosition.Y);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_WithBlockedCells_ParsesCorrectly()
		{
			// Arrange
			string content = "o+o\no+o\noox";
			string filePath = Path.Combine(tempDirectory, "blocked_grid.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act
			var exercise = parser.ParseFromFile(filePath);

			// Assert
			Assert.NotNull(exercise);
			Assert.True(exercise.Grid.IsCellBlocked(1, 1));
			Assert.True(exercise.Grid.IsCellBlocked(1, 2));
			Assert.False(exercise.Grid.IsCellBlocked(0, 0));
			Assert.False(exercise.Grid.IsCellBlocked(2, 0));

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_FileNotFound_ThrowsException()
		{
			// Arrange
			var parser = new GridFileParser();
			string nonExistentPath = Path.Combine(tempDirectory, "nonexistent.txt");

			// Act & Assert
			Assert.Throws<FileNotFoundException>(() => parser.ParseFromFile(nonExistentPath));
		}

		[Fact]
		public void ParseFromFile_EmptyFile_ThrowsException()
		{
			// Arrange
			string filePath = Path.Combine(tempDirectory, "empty_grid.txt");
			File.WriteAllText(filePath, "");

			var parser = new GridFileParser();

			// Act & Assert
			Assert.Throws<FormatException>(() => parser.ParseFromFile(filePath));

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_NoEndPosition_ThrowsException()
		{
			// Arrange
			string content = "ooo\nooo\nooo";
			string filePath = Path.Combine(tempDirectory, "no_end_grid.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act & Assert
			var exception = Assert.Throws<FormatException>(() => parser.ParseFromFile(filePath));
			Assert.Contains("end position", exception.Message);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_InvalidCharacter_ThrowsException()
		{
			// Arrange
			string content = "ooo\noxo\noZo";
			string filePath = Path.Combine(tempDirectory, "invalid_char_grid.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act & Assert
			var exception = Assert.Throws<FormatException>(() => parser.ParseFromFile(filePath));
			Assert.Contains("Invalid character", exception.Message);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_InconsistentRowLength_ThrowsException()
		{
			// Arrange
			string content = "ooo\noo\noox";
			string filePath = Path.Combine(tempDirectory, "inconsistent_grid.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act & Assert
			var exception = Assert.Throws<FormatException>(() => parser.ParseFromFile(filePath));
			Assert.Contains("same length", exception.Message);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_SingleCellGrid_ParsesCorrectly()
		{
			// Arrange
			string content = "x";
			string filePath = Path.Combine(tempDirectory, "single_cell_grid.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act
			var exercise = parser.ParseFromFile(filePath);

			// Assert
			Assert.NotNull(exercise);
			Assert.Equal(1, exercise.Grid.Width);
			Assert.Equal(1, exercise.Grid.Height);
			Assert.Equal(0, exercise.Grid.EndPosition.X);
			Assert.Equal(0, exercise.Grid.EndPosition.Y);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_LargeGrid_ParsesCorrectly()
		{
			// Arrange
			string content = "ooooo\nooooo\nooooo\nooooo\noooox";
			string filePath = Path.Combine(tempDirectory, "large_grid.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act
			var exercise = parser.ParseFromFile(filePath);

			// Assert
			Assert.NotNull(exercise);
			Assert.Equal(5, exercise.Grid.Width);
			Assert.Equal(5, exercise.Grid.Height);
			Assert.Equal(4, exercise.Grid.EndPosition.X);
			Assert.Equal(0, exercise.Grid.EndPosition.Y);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_WithBlankLines_IgnoresBlanks()
		{
			// Arrange
			string content = "\nooo\n\nooo\noox\n";
			string filePath = Path.Combine(tempDirectory, "blank_lines_grid.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act
			var exercise = parser.ParseFromFile(filePath);

			// Assert
			Assert.NotNull(exercise);
			Assert.Equal(3, exercise.Grid.Width);
			Assert.Equal(3, exercise.Grid.Height);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_EndPositionAtTopLeft_ParsesCorrectly()
		{
			// Arrange
			string content = "xoo\nooo\nooo";
			string filePath = Path.Combine(tempDirectory, "top_left_grid.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act
			var exercise = parser.ParseFromFile(filePath);

			// Assert
			Assert.NotNull(exercise);
			Assert.Equal(0, exercise.Grid.EndPosition.X);
			Assert.Equal(2, exercise.Grid.EndPosition.Y); // Top in file = Y=2 in grid

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_ComplexMaze_ParsesCorrectly()
		{
			// Arrange
			string content = "o++o\no++o\nooox";
			string filePath = Path.Combine(tempDirectory, "maze_grid.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act
			var exercise = parser.ParseFromFile(filePath);

			// Assert
			Assert.NotNull(exercise);
			Assert.Equal(4, exercise.Grid.Width);
			Assert.Equal(3, exercise.Grid.Height);
			
			// Check blocked cells (Y coordinates are flipped)
			Assert.True(exercise.Grid.IsCellBlocked(1, 1));
			Assert.True(exercise.Grid.IsCellBlocked(2, 1));
			Assert.True(exercise.Grid.IsCellBlocked(1, 2));
			Assert.True(exercise.Grid.IsCellBlocked(2, 2));

			// Check end position
			Assert.Equal(3, exercise.Grid.EndPosition.X);
			Assert.Equal(0, exercise.Grid.EndPosition.Y);

			// Cleanup
			File.Delete(filePath);
		}

		[Fact]
		public void ParseFromFile_SetsExerciseName_FromFileName()
		{
			// Arrange
			string content = "oox";
			string filePath = Path.Combine(tempDirectory, "MyExercise.txt");
			File.WriteAllText(filePath, content);

			var parser = new GridFileParser();

			// Act
			var exercise = parser.ParseFromFile(filePath);

			// Assert
			Assert.Equal("MyExercise", exercise.Name);

			// Cleanup
			File.Delete(filePath);
		}
	}
}
