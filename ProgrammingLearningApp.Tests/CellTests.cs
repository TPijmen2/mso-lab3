using ProgrammingLearningApp.Models;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class CellTests
	{
		[Fact]
		public void Cell_DefaultConstructor_CreatesOpenCell()
		{
			// Act
			var cell = new Cell();

			// Assert
			Assert.False(cell.IsBlocked);
			Assert.False(cell.IsEndPosition);
			Assert.False(cell.IsVisited);
		}

		[Fact]
		public void Cell_ConstructorWithBlocked_SetsBlockedState()
		{
			// Act
			var cell = new Cell(isBlocked: true);

			// Assert
			Assert.True(cell.IsBlocked);
		}

		[Fact]
		public void Reset_ClearsVisitedFlag()
		{
			// Arrange
			var cell = new Cell { IsVisited = true };

			// Act
			cell.Reset();

			// Assert
			Assert.False(cell.IsVisited);
		}
	}
}