using Microsoft.VisualStudio.TestPlatform.TestHost;
using ProgrammingLearningApp;
using System.Windows.Input;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class ProgramTests
	{
		[Fact]
		public void Execute_BasicProgram_ReturnsCorrectResult()
		{
			// Arrange
			var program = ProgramFactory.CreateBasicProgram();

			// Act
			var result = program.Execute();

			// Assert
			Assert.Equal(0, result.FinalPosition.X);
			Assert.Equal(0, result.FinalPosition.Y);
			Assert.Equal(Direction.East, result.FinalDirection);
			Assert.Equal(8, result.Trace.Count);
		}

		[Fact]
		public void Execute_AdvancedProgram_ReturnsCorrectResult()
		{
			// Arrange
			var program = ProgramFactory.CreateAdvancedProgram();

			// Act
			var result = program.Execute();

			// Assert
			Assert.Equal(0, result.FinalPosition.X);
			Assert.Equal(0, result.FinalPosition.Y);
			Assert.Equal(Direction.East, result.FinalDirection);
		}

		[Fact]
		public void CalculateMetrics_BasicProgram_ReturnsCorrectMetrics()
		{
			// Arrange
			var program = ProgramFactory.CreateBasicProgram();

			// Act
			var metrics = program.CalculateMetrics();

			// Assert
			Assert.Equal(8, metrics.CommandCount);
			Assert.Equal(0, metrics.MaxNestingLevel);
			Assert.Equal(0, metrics.RepeatCount);
		}

		[Fact]
		public void CalculateMetrics_AdvancedProgram_ReturnsCorrectMetrics()
		{
			// Arrange
			var program = ProgramFactory.CreateAdvancedProgram();

			// Act
			var metrics = program.CalculateMetrics();

			// Assert
			Assert.Equal(3, metrics.CommandCount);
			Assert.Equal(1, metrics.MaxNestingLevel);
			Assert.Equal(1, metrics.RepeatCount);
		}

		[Fact]
		public void CalculateMetrics_ExpertProgram_ReturnsCorrectMetrics()
		{
			// Arrange
			var program = ProgramFactory.CreateExpertProgram();

			// Act
			var metrics = program.CalculateMetrics();

			// Assert
			Assert.Equal(11, metrics.CommandCount);
			Assert.Equal(1, metrics.MaxNestingLevel);
			Assert.Equal(2, metrics.RepeatCount);
		}

		[Fact]
		public void GetTextRepresentation_ReturnsCorrectFormat()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(5),
				new TurnCommand(TurnDirection.Right)
			};
			var program = new Program("Test", commands);

			// Act
			string text = program.GetTextRepresentation();

			// Assert
			Assert.Contains("Move 5", text);
			Assert.Contains("Turn right", text);
		}
	}
}