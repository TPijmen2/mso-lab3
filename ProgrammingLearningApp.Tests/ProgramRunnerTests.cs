using ProgrammingLearningApp.Commands;
using ProgrammingLearningApp.Exceptions;
using ProgrammingLearningApp.Models;
using ProgrammingLearningApp.Services;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class ProgramRunnerTests
	{
		[Fact]
		public void ProgramRunner_Constructor_NullProgram_ThrowsException()
		{
			// Act & Assert
			Assert.Throws<ArgumentNullException>(() => new ProgramRunner(null));
		}

		[Fact]
		public void ProgramRunner_Execute_SimpleProgram_ReturnsSuccess()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(5),
				new TurnCommand(TurnDirection.Right),
				new MoveCommand(3)
			};
			var program = new Program("Test", commands);
			var runner = new ProgramRunner(program);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.Equal(ExecutionStatus.Success, result.Status);
			Assert.Equal(3, result.Trace.Count);
			Assert.Contains("Move 5", result.Trace);
			Assert.Contains("Turn right", result.Trace);
			Assert.Contains("Move 3", result.Trace);
		}

		[Fact]
		public void ProgramRunner_Execute_WithExercise_ReturnsSuccessWhenCompleted()
		{
			// Arrange
			var grid = new Grid(5, 5);
			grid.SetEndPosition(3, 0);
			var exercise = new PathfindingExercise("Test", grid, new Position(0, 0));

			var commands = new List<ICommand>
			{
				new MoveCommand(3)
			};
			var program = new Program("Test", commands);
			var runner = new ProgramRunner(program, exercise);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.Equal(ExecutionStatus.Success, result.Status);
			Assert.Equal(3, result.FinalPosition.X);
			Assert.Equal(0, result.FinalPosition.Y);
		}

		[Fact]
		public void ProgramRunner_Execute_WithExercise_ReturnsFailureWhenNotCompleted()
		{
			// Arrange
			var grid = new Grid(5, 5);
			grid.SetEndPosition(4, 4);
			var exercise = new PathfindingExercise("Test", grid, new Position(0, 0));

			var commands = new List<ICommand>
			{
				new MoveCommand(2)
			};
			var program = new Program("Test", commands);
			var runner = new ProgramRunner(program, exercise);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.Equal(ExecutionStatus.Failure, result.Status);
			Assert.NotNull(result.ErrorMessage);
			Assert.Contains("did not reach the end position", result.ErrorMessage);
		}

		[Fact]
		public void ProgramRunner_Execute_OutOfBounds_ReturnsRuntimeError()
		{
			// Arrange
			var grid = new Grid(5, 5);
			grid.SetEndPosition(4, 4);
			var exercise = new PathfindingExercise("Test", grid, new Position(0, 0));

			var commands = new List<ICommand>
			{
				new MoveCommand(10) // This will go out of bounds
			};
			var program = new Program("Test", commands);
			var runner = new ProgramRunner(program, exercise);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.Equal(ExecutionStatus.RuntimeError, result.Status);
			Assert.NotNull(result.ErrorMessage);
			Assert.Contains("Out of bounds", result.ErrorMessage);
			Assert.IsType<OutOfBoundsException>(result.Exception);
		}

		[Fact]
		public void ProgramRunner_Execute_BlockedCell_ReturnsRuntimeError()
		{
			// Arrange
			var grid = new Grid(5, 5);
			grid.SetEndPosition(4, 0);
			grid.SetCellBlocked(2, 0, true);
			var exercise = new PathfindingExercise("Test", grid, new Position(0, 0));

			var commands = new List<ICommand>
			{
				new MoveCommand(3) // Will hit blocked cell at position 2
			};
			var program = new Program("Test", commands);
			var runner = new ProgramRunner(program, exercise);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.Equal(ExecutionStatus.RuntimeError, result.Status);
			Assert.NotNull(result.ErrorMessage);
			Assert.Contains("Blocked cell", result.ErrorMessage);
			Assert.IsType<BlockedCellException>(result.Exception);
		}

		[Fact]
		public void ProgramRunner_Execute_WithRepeat_ExecutesCorrectly()
		{
			// Arrange
			var innerCommands = new List<ICommand>
			{
				new MoveCommand(1),
				new TurnCommand(TurnDirection.Right)
			};
			var commands = new List<ICommand>
			{
				new RepeatCommand(3, innerCommands)
			};
			var program = new Program("Test", commands);
			var runner = new ProgramRunner(program);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.Equal(ExecutionStatus.Success, result.Status);
			Assert.Equal(6, result.Trace.Count); // 3 iterations × 2 commands
		}

		[Fact]
		public void ProgramRunner_Execute_WithRepeatUntilWallAhead_StopsAtWall()
		{
			// Arrange
			var grid = new Grid(5, 1);
			grid.SetCellBlocked(3, 0, true);
			grid.SetEndPosition(2, 0);
			var exercise = new PathfindingExercise("Test", grid, new Position(0, 0));

			var innerCommands = new List<ICommand>
			{
				new MoveCommand(1)
			};
			var commands = new List<ICommand>
			{
				new RepeatUntilCommand(Condition.WallAhead, innerCommands)
			};
			var program = new Program("Test", commands);
			var runner = new ProgramRunner(program, exercise);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.Equal(ExecutionStatus.Success, result.Status);
			Assert.Equal(2, result.FinalPosition.X); // Stops before wall at position 3
		}

		[Fact]
		public void ProgramRunner_Execute_WithRepeatUntilGridEdge_StopsAtEdge()
		{
			// Arrange
			var grid = new Grid(5, 1);
			grid.SetEndPosition(4, 0);
			var exercise = new PathfindingExercise("Test", grid, new Position(0, 0));

			var innerCommands = new List<ICommand>
			{
				new MoveCommand(1)
			};
			var commands = new List<ICommand>
			{
				new RepeatUntilCommand(Condition.GridEdge, innerCommands)
			};
			var program = new Program("Test", commands);
			var runner = new ProgramRunner(program, exercise);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.Equal(ExecutionStatus.Success, result.Status);
			Assert.Equal(4, result.FinalPosition.X); // Stops at edge
		}

		[Fact]
		public void ProgramRunner_Execute_EmptyProgram_ReturnsSuccess()
		{
			// Arrange
			var program = new Program("Empty", new List<ICommand>());
			var runner = new ProgramRunner(program);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.Equal(ExecutionStatus.Success, result.Status);
			Assert.Empty(result.Trace);
			Assert.Equal(0, result.FinalPosition.X);
			Assert.Equal(0, result.FinalPosition.Y);
			Assert.Equal(Direction.East, result.FinalDirection);
		}

		[Fact]
		public void ProgramRunner_Execute_RecordsFinalDirection()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new TurnCommand(TurnDirection.Right),
				new TurnCommand(TurnDirection.Right)
			};
			var program = new Program("Test", commands);
			var runner = new ProgramRunner(program);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.Equal(ExecutionStatus.Success, result.Status);
			Assert.Equal(Direction.West, result.FinalDirection);
		}

		[Fact]
		public void ProgramRunner_Execute_IsSuccess_ReturnsTrue()
		{
			// Arrange
			var program = new Program("Test", new List<ICommand>());
			var runner = new ProgramRunner(program);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.True(result.IsSuccess);
		}

		[Fact]
		public void ProgramRunner_Execute_WithError_IsSuccess_ReturnsFalse()
		{
			// Arrange
			var grid = new Grid(3, 3);
			grid.SetEndPosition(2, 2);
			var exercise = new PathfindingExercise("Test", grid, new Position(0, 0));

			var commands = new List<ICommand>
			{
				new MoveCommand(10) // Out of bounds
			};
			var program = new Program("Test", commands);
			var runner = new ProgramRunner(program, exercise);

			// Act
			var result = runner.Execute();

			// Assert
			Assert.False(result.IsSuccess);
		}
	}
}
