using ProgrammingLearningApp.Models;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class PathfindingExerciseTests
	{
		[Fact]
		public void Exercise_Constructor_SetsProperties()
		{
			// Arrange
			var grid = new Grid(5, 5);
			grid.SetEndPosition(4, 4);
			var startPos = new Position(0, 0);

			// Act
			var exercise = new PathfindingExercise("Test", grid, startPos);

			// Assert
			Assert.Equal("Test", exercise.Name);
			Assert.Equal(grid, exercise.Grid);
			Assert.Equal(startPos, exercise.StartPosition);
		}

		[Fact]
		public void Exercise_Constructor_DefaultsToBottomLeft()
		{
			// Arrange
			var grid = new Grid(5, 5);
			grid.SetEndPosition(4, 4);

			// Act
			var exercise = new PathfindingExercise("Test", grid);

			// Assert
			// Default start position is bottom-left corner: (0, Grid.Height - 1)
			Assert.Equal(0, exercise.StartPosition.X);
			Assert.Equal(4, exercise.StartPosition.Y); // Height - 1 = 5 - 1 = 4
		}

		[Fact]
		public void Exercise_Constructor_ThrowsOnNullGrid()
		{
			// Act & Assert
			Assert.Throws<ArgumentNullException>(() =>
				new PathfindingExercise("Test", null));
		}

		[Fact]
		public void Exercise_Constructor_ThrowsOnEmptyName()
		{
			// Arrange
			var grid = new Grid(5, 5);

			// Act & Assert
			Assert.Throws<ArgumentException>(() =>
				new PathfindingExercise("", grid));
		}

		[Fact]
		public void IsCompleted_AtEndPosition_ReturnsTrue()
		{
			// Arrange
			var grid = new Grid(5, 5);
			grid.SetEndPosition(4, 4);
			var exercise = new PathfindingExercise("Test", grid);

			// Act
			bool completed = exercise.IsCompleted(new Position(4, 4));

			// Assert
			Assert.True(completed);
		}

		[Fact]
		public void IsCompleted_NotAtEndPosition_ReturnsFalse()
		{
			// Arrange
			var grid = new Grid(5, 5);
			grid.SetEndPosition(4, 4);
			var exercise = new PathfindingExercise("Test", grid);

			// Act
			bool completed = exercise.IsCompleted(new Position(2, 2));

			// Assert
			Assert.False(completed);
		}
	}
}