using ProgrammingLearningApp.Commands;
using ProgrammingLearningApp.Models;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class RepeatUntilCommandTests
	{
		[Fact]
		public void RepeatUntil_EmptyCommandList_ThrowsException()
		{
			// Arrange
			var emptyList = new List<ICommand>();

			// Act & Assert
			Assert.Throws<ArgumentException>(() =>
				new RepeatUntilCommand(Condition.WallAhead, emptyList));
		}

		[Fact]
		public void RepeatUntil_WallAhead_StopsAtWall()
		{
			// Arrange
			var grid = new Grid(5, 1);
			grid.SetCellBlocked(3, 0, true);

			var character = new Character(grid);
			var commands = new List<ICommand> { new MoveCommand(1) };
			var repeatUntil = new RepeatUntilCommand(Condition.WallAhead, commands);

			// Act
			repeatUntil.Execute(character);

			// Assert - Should stop at position (2, 0), before the wall at (3, 0)
			Assert.Equal(2, character.Position.X);
		}

		[Fact]
		public void RepeatUntil_GridEdge_StopsAtEdge()
		{
			// Arrange
			var grid = new Grid(5, 1);
			var character = new Character(grid);
			var commands = new List<ICommand> { new MoveCommand(1) };
			var repeatUntil = new RepeatUntilCommand(Condition.GridEdge, commands);

			// Act
			repeatUntil.Execute(character);

			// Assert - Should stop at position (4, 0), at the edge
			Assert.Equal(4, character.Position.X);
		}

		[Fact]
		public void RepeatUntil_NoGrid_ThrowsInfiniteLoopException()
		{
			// Arrange
			var character = new Character(); // No grid
			var commands = new List<ICommand> { new MoveCommand(1) };
			var repeatUntil = new RepeatUntilCommand(Condition.WallAhead, commands);

			// Act & Assert
			// Without grid, condition is always false, so it loops until max iterations
			// and throws InvalidOperationException
			Assert.Throws<InvalidOperationException>(() => repeatUntil.Execute(character));
		}

		[Fact]
		public void GetCommandCount_IncludesNestedCommands()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(1),
				new TurnCommand(TurnDirection.Left)
			};
			var repeatUntil = new RepeatUntilCommand(Condition.WallAhead, commands);

			// Act
			int count = repeatUntil.GetCommandCount();

			// Assert
			Assert.Equal(3, count); // 1 RepeatUntil + 2 nested commands
		}

		[Fact]
		public void GetMaxNestingLevel_ReturnsCorrectLevel()
		{
			// Arrange
			var innerCommands = new List<ICommand> { new MoveCommand(1) };
			var innerRepeat = new RepeatCommand(2, innerCommands);
			var outerCommands = new List<ICommand> { innerRepeat };
			var repeatUntil = new RepeatUntilCommand(Condition.WallAhead, outerCommands);

			// Act
			int level = repeatUntil.GetMaxNestingLevel();

			// Assert
			Assert.Equal(2, level); // RepeatUntil contains Repeat
		}

		[Fact]
		public void GetRepeatCount_CountsItself()
		{
			// Arrange
			var commands = new List<ICommand> { new MoveCommand(1) };
			var repeatUntil = new RepeatUntilCommand(Condition.WallAhead, commands);

			// Act
			int count = repeatUntil.GetRepeatCount();

			// Assert
			Assert.Equal(1, count);
		}

		[Fact]
		public void ToString_FormatsCorrectly()
		{
			// Arrange
			var commands = new List<ICommand> { new MoveCommand(1) };
			var repeatUntil = new RepeatUntilCommand(Condition.WallAhead, commands);

			// Act
			string result = repeatUntil.ToString();

			// Assert
			Assert.Contains("RepeatUntil WallAhead", result);
			Assert.Contains("Move 1", result);
		}
	}
}