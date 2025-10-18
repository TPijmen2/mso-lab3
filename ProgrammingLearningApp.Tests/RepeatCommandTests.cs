using Microsoft.VisualStudio.TestPlatform.TestHost;
using ProgrammingLearningApp;
using System.Windows.Input;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class RepeatCommandTests
	{
		[Fact]
		public void Execute_RepeatsCommandsCorrectly()
		{
			// Arrange
			var character = new Character();
			var commands = new List<ICommand>
			{
				new MoveCommand(1),
				new TurnCommand(TurnDirection.Right)
			};
			var repeat = new RepeatCommand(3, commands);

			// Act
			repeat.Execute(character);

			// Assert
			Assert.Equal(0, character.Position.X);
			Assert.Equal(-1, character.Position.Y);
			Assert.Equal(Direction.North, character.Direction);
		}

		[Fact]
		public void GetCommandCount_CountsAllCommands()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(1),
				new TurnCommand(TurnDirection.Right)
			};
			var repeat = new RepeatCommand(3, commands);

			// Act
			int count = repeat.GetCommandCount();

			// Assert
			Assert.Equal(3, count); // 1 repeat + 2 inner commands
		}

		[Fact]
		public void GetMaxNestingLevel_SingleLevel_ReturnsOne()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(1)
			};
			var repeat = new RepeatCommand(2, commands);

			// Act
			int level = repeat.GetMaxNestingLevel();

			// Assert
			Assert.Equal(1, level);
		}

		[Fact]
		public void GetMaxNestingLevel_NestedRepeat_ReturnsTwo()
		{
			// Arrange
			var innerCommands = new List<ICommand>
			{
				new MoveCommand(1)
			};
			var innerRepeat = new RepeatCommand(2, innerCommands);

			var outerCommands = new List<ICommand>
			{
				innerRepeat
			};
			var outerRepeat = new RepeatCommand(3, outerCommands);

			// Act
			int level = outerRepeat.GetMaxNestingLevel();

			// Assert
			Assert.Equal(2, level);
		}

		[Fact]
		public void GetRepeatCount_SingleRepeat_ReturnsOne()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(1)
			};
			var repeat = new RepeatCommand(2, commands);

			// Act
			int count = repeat.GetRepeatCount();

			// Assert
			Assert.Equal(1, count);
		}

		[Fact]
		public void GetRepeatCount_NestedRepeat_ReturnsTwo()
		{
			// Arrange
			var innerCommands = new List<ICommand>
			{
				new MoveCommand(1)
			};
			var innerRepeat = new RepeatCommand(2, innerCommands);

			var outerCommands = new List<ICommand>
			{
				innerRepeat
			};
			var outerRepeat = new RepeatCommand(3, outerCommands);

			// Act
			int count = outerRepeat.GetRepeatCount();

			// Assert
			Assert.Equal(2, count);
		}
	}
}