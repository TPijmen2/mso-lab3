using ProgrammingLearningApp.Commands;
using ProgrammingLearningApp.Exceptions;
using ProgrammingLearningApp.Models;

namespace ProgrammingLearningApp.Services
{
	public enum ExecutionStatus
	{
		Success,
		Failure,
		RuntimeError
	}

	public class ProgramExecutionResult
	{
		public ExecutionStatus Status { get; set; }
		public List<string> Trace { get; set; }
		public Position FinalPosition { get; set; }
		public Direction FinalDirection { get; set; }
		public string ErrorMessage { get; set; }
		public Exception Exception { get; set; }

		public ProgramExecutionResult()
		{
			Trace = new List<string>();
			Status = ExecutionStatus.Success;
		}

		public bool IsSuccess => Status == ExecutionStatus.Success;
	}

	public class ProgramRunner
	{
		private readonly Program program;
		private readonly PathfindingExercise exercise;

		public ProgramRunner(Program program, PathfindingExercise exercise = null)
		{
			this.program = program ?? throw new ArgumentNullException(nameof(program));
			this.exercise = exercise;
			
			LoggingService.Instance.LogDebug($"ProgramRunner created for program: {program.Name}");
			if (exercise != null)
			{
				LoggingService.Instance.LogDebug($"Exercise loaded: {exercise.Name}");
			}
		}

		public ProgramExecutionResult Execute()
		{
			LoggingService.Instance.LogInfo($"Starting program execution: {program.Name}");
			
			var result = new ProgramExecutionResult();
			Character character;

			// Create character with or without grid
			if (exercise != null)
			{
				character = new Character(exercise.Grid);
				character.Position = exercise.StartPosition.Copy();
				character.Path.Clear();
				character.Path.Add(character.Position.Copy());
				LoggingService.Instance.LogDebug($"Character initialized at position: {character.Position}");
			}
			else
			{
				character = new Character();
				LoggingService.Instance.LogDebug("Character initialized without grid");
			}

			try
			{
				// Execute program
				ExecuteCommands(program.Commands, character, result.Trace);

				// Check if exercise is completed
				if (exercise != null)
				{
					if (exercise.IsCompleted(character.Position))
					{
						result.Status = ExecutionStatus.Success;
						LoggingService.Instance.LogInfo($"Exercise completed successfully! Final position: {character.Position}");
					}
					else
					{
						result.Status = ExecutionStatus.Failure;
						result.ErrorMessage = $"Character did not reach the end position. " +
											$"Current: {character.Position}, Target: {exercise.Grid.EndPosition}";
						LoggingService.Instance.LogWarning($"Exercise failed: {result.ErrorMessage}");
					}
				}
				else
				{
					result.Status = ExecutionStatus.Success;
					LoggingService.Instance.LogInfo("Program executed successfully (no exercise)");
				}

				result.FinalPosition = character.Position.Copy();
				result.FinalDirection = character.Direction;
			}
			catch (OutOfBoundsException ex)
			{
				result.Status = ExecutionStatus.RuntimeError;
				result.ErrorMessage = $"Out of bounds error: {ex.Message}";
				result.Exception = ex;
				result.FinalPosition = character.Position.Copy();
				result.FinalDirection = character.Direction;
				
				LoggingService.Instance.LogError($"OutOfBoundsException during execution: {ex.Message}");
				LoggingService.Instance.LogDebug($"Position at error: {character.Position}");
			}
			catch (BlockedCellException ex)
			{
				result.Status = ExecutionStatus.RuntimeError;
				result.ErrorMessage = $"Blocked cell error: {ex.Message}";
				result.Exception = ex;
				result.FinalPosition = character.Position.Copy();
				result.FinalDirection = character.Direction;
				
				LoggingService.Instance.LogError($"BlockedCellException during execution: {ex.Message}");
				LoggingService.Instance.LogDebug($"Position at error: {character.Position}");
			}
			catch (Exception ex)
			{
				result.Status = ExecutionStatus.RuntimeError;
				result.ErrorMessage = $"Runtime error: {ex.Message}";
				result.Exception = ex;
				if (character != null)
				{
					result.FinalPosition = character.Position?.Copy();
					result.FinalDirection = character.Direction;
				}
				
				LoggingService.Instance.LogException(ex, "Unexpected exception during program execution");
			}

			LoggingService.Instance.LogInfo($"Program execution completed with status: {result.Status}");
			return result;
		}

		private void ExecuteCommands(List<ICommand> commands, Character character, List<string> trace)
		{
			foreach (var command in commands)
			{
				if (command is RepeatCommand repeatCmd)
				{
					LoggingService.Instance.LogDebug($"Executing Repeat {repeatCmd.Times} times");
					for (int i = 0; i < repeatCmd.Times; i++)
					{
						ExecuteCommands(repeatCmd.Commands, character, trace);
					}
				}
				else if (command is RepeatUntilCommand repeatUntilCmd)
				{
					// Execute and trace the RepeatUntil itself
					int iterations = 0;
					const int maxIterations = 10000;

					LoggingService.Instance.LogDebug($"Executing RepeatUntil with condition: {repeatUntilCmd.Condition}");
					
					while (!EvaluateCondition(repeatUntilCmd.Condition, character) && iterations < maxIterations)
					{
						ExecuteCommands(repeatUntilCmd.Commands, character, trace);
						iterations++;
					}

					if (iterations >= maxIterations)
					{
						LoggingService.Instance.LogError($"RepeatUntil exceeded maximum iterations ({maxIterations})");
						throw new InvalidOperationException("RepeatUntil exceeded maximum iterations");
					}
					
					LoggingService.Instance.LogDebug($"RepeatUntil completed after {iterations} iterations");
				}
				else
				{
					command.Execute(character);

					// Add to trace
					if (command is MoveCommand moveCmd)
						trace.Add($"Move {moveCmd.Steps}");
					else if (command is TurnCommand turnCmd)
					{
						string dir = turnCmd.TurnDirection == TurnDirection.Left ? "left" : "right";
						trace.Add($"Turn {dir}");
					}
				}
			}
		}

		private bool EvaluateCondition(Condition condition, Character character)
		{
			if (character.Grid == null)
				return false;

			switch (condition)
			{
				case Condition.WallAhead:
					return IsWallAhead(character);
				case Condition.GridEdge:
					return IsAtGridEdge(character);
				default:
					return false;
			}
		}

		private bool IsWallAhead(Character character)
		{
			var nextPos = character.Grid.GetNextPosition(character.Position, character.Direction);
			if (!character.Grid.IsWithinBounds(nextPos))
				return true;
			return character.Grid.IsCellBlocked(nextPos);
		}

		private bool IsAtGridEdge(Character character)
		{
			var nextPos = character.Grid.GetNextPosition(character.Position, character.Direction);
			return !character.Grid.IsWithinBounds(nextPos);
		}
	}
}