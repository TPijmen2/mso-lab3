using Microsoft.VisualStudio.TestPlatform.TestHost;
using ProgrammingLearningApp;
using System.Windows.Input;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class TurnCommandTests
	{
		[Fact]
		public void Execute_TurnLeft_ChangesDirection()
		{
			// Arrange
			var character = new Character();
			var command = new TurnCommand(TurnDirection.Left);

			// Act
			command.Execute(character);

			// Assert
			Assert.Equal(Direction.North, character.Direction);
		}

		[Fact]
		public void Execute_TurnRight_ChangesDirection()
		{
			// Arrange
			var character = new Character();
			var command = new TurnCommand(TurnDirection.Right);

			// Act
			command.Execute(character);

			// Assert
			Assert.Equal(Direction.South, character.Direction);
		}

		[Fact]
		public void GetCommandCount_ReturnsOne()
		{
			// Arrange
			var command = new TurnCommand(TurnDirection.Left);

			// Act
			int count = command.GetCommandCount();

			// Assert
			Assert.Equal(1, count);
		}

		[Fact]
		public void ToString_TurnLeft_ReturnsCorrectFormat()
		{
			// Arrange
			var command = new TurnCommand(TurnDirection.Left);

			// Act
			string result = command.ToString();

			// Assert
			Assert.Equal("Turn left", result);
		}

		[Fact]
		public void ToString_TurnRight_ReturnsCorrectFormat()
		{
			// Arrange
			var command = new TurnCommand(TurnDirection.Right);

			// Act
			string result = command.ToString();

			// Assert
			Assert.Equal("Turn right", result);
		}
	}
}