using ProgrammingLearningApp.Models;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class GridTests
	{
		[Fact]
		public void Grid_Constructor_CreatesCorrectDimensions()
		{
			// Arrange & Act
			var grid = new Grid(5, 10);

			// Assert
			Assert.Equal(5, grid.Width);
			Assert.Equal(10, grid.Height);
		}

		[Theory]
		[InlineData(0, 5)]
		[InlineData(5, 0)]
		[InlineData(-1, 5)]
		[InlineData(5, -1)]
		public void Grid_Constructor_ThrowsOnInvalidDimensions(int width, int height)
		{
			// Act & Assert
			Assert.Throws<ArgumentException>(() => new Grid(width, height));
		}

		[Fact]
		public void GetCell_ValidPosition_ReturnsCell()
		{
			// Arrange
			var grid = new Grid(5, 5);

			// Act
			var cell = grid.GetCell(2, 3);

			// Assert
			Assert.NotNull(cell);
			Assert.False(cell.IsBlocked);
		}

		[Theory]
		[InlineData(-1, 0)]
		[InlineData(0, -1)]
		[InlineData(5, 0)]
		[InlineData(0, 5)]
		public void GetCell_OutOfBounds_ThrowsException(int x, int y)
		{
			// Arrange
			var grid = new Grid(5, 5);

			// Act & Assert
			Assert.Throws<ArgumentOutOfRangeException>(() => grid.GetCell(x, y));
		}

		[Fact]
		public void SetCellBlocked_ValidPosition_BlocksCell()
		{
			// Arrange
			var grid = new Grid(5, 5);

			// Act
			grid.SetCellBlocked(2, 3, true);

			// Assert
			Assert.True(grid.GetCell(2, 3).IsBlocked);
		}

		[Fact]
		public void SetEndPosition_ValidPosition_SetsEndPosition()
		{
			// Arrange
			var grid = new Grid(5, 5);

			// Act
			grid.SetEndPosition(4, 4);

			// Assert
			Assert.Equal(new Position(4, 4), grid.EndPosition);
			Assert.True(grid.GetCell(4, 4).IsEndPosition);
		}

		[Theory]
		[InlineData(0, 0, true)]
		[InlineData(4, 4, true)]
		[InlineData(5, 5, false)]
		[InlineData(-1, 0, false)]
		public void IsWithinBounds_VariousPositions_ReturnsCorrectResult(int x, int y, bool expected)
		{
			// Arrange
			var grid = new Grid(5, 5);

			// Act
			bool result = grid.IsWithinBounds(x, y);

			// Assert
			Assert.Equal(expected, result);
		}

		[Fact]
		public void IsCellBlocked_BlockedCell_ReturnsTrue()
		{
			// Arrange
			var grid = new Grid(5, 5);
			grid.SetCellBlocked(2, 2, true);

			// Act & Assert
			Assert.True(grid.IsCellBlocked(2, 2));
		}

		[Fact]
		public void IsCellBlocked_OutOfBounds_ReturnsTrue()
		{
			// Arrange
			var grid = new Grid(5, 5);

			// Act & Assert
			Assert.True(grid.IsCellBlocked(10, 10));
		}

		[Fact]
		public void GetNextPosition_NorthDirection_ReturnsCorrectPosition()
		{
			// Arrange
			var grid = new Grid(5, 5);
			var current = new Position(2, 2);

			// Act
			var next = grid.GetNextPosition(current, Direction.North);

			// Assert
			Assert.Equal(2, next.X);
			Assert.Equal(3, next.Y);
		}

		[Fact]
		public void Reset_ClearsVisitedFlags()
		{
			// Arrange
			var grid = new Grid(5, 5);
			grid.GetCell(1, 1).IsVisited = true;
			grid.GetCell(2, 2).IsVisited = true;

			// Act
			grid.Reset();

			// Assert
			Assert.False(grid.GetCell(1, 1).IsVisited);
			Assert.False(grid.GetCell(2, 2).IsVisited);
		}
	}
}