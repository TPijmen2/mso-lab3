using Microsoft.VisualStudio.TestPlatform.TestHost;
using ProgrammingLearningApp;
using System.Windows.Input;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class MoveCommandTests
	{
		[Fact]
		public void Execute_MovesCharacter()
		{
			// Arrange
			var character = new Character();
			var command = new MoveCommand(3);

			// Act
			command.Execute(character);

			// Assert
			Assert.Equal(3, character.Position.X);
		}

		[Fact]
		public void GetCommandCount_ReturnsOne()
		{
			// Arrange
			var command = new MoveCommand(5);

			// Act
			int count = command.GetCommandCount();

			// Assert
			Assert.Equal(1, count);
		}

		[Fact]
		public void GetMaxNestingLevel_ReturnsZero()
		{
			// Arrange
			var command = new MoveCommand(5);

			// Act
			int level = command.GetMaxNestingLevel();

			// Assert
			Assert.Equal(0, level);
		}

		[Fact]
		public void ToString_ReturnsCorrectFormat()
		{
			// Arrange
			var command = new MoveCommand(10);

			// Act
			string result = command.ToString();

			// Assert
			Assert.Equal("Move 10", result);
		}
	}
}