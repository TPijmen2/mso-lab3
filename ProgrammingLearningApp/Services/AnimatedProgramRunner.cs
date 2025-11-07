using ProgrammingLearningApp.Commands;
using ProgrammingLearningApp.Exceptions;
using ProgrammingLearningApp.Models;

namespace ProgrammingLearningApp.Services
{
	public class AnimatedProgramRunner : ProgramRunner
	{
		private List<AnimationStep> animationSteps;
		private int stepCounter;

		public AnimatedProgramRunner(Program program, PathfindingExercise exercise = null)
			: base(program, exercise)
		{
			animationSteps = new List<AnimationStep>();
			stepCounter = 0;
		}

		public new ProgramExecutionResult Execute()
		{
			LoggingService.Instance.LogInfo($"Starting animated program execution");
			animationSteps.Clear();
			stepCounter = 0;

			var result = new ProgramExecutionResult();
			Character character;

			// Create character with or without grid
			if (exercise != null)
			{
				character = new Character(exercise.Grid);
				character.Position = exercise.StartPosition.Copy();
				character.Path.Clear();
				character.Path.Add(character.Position.Copy());
				
				// Add initial step
				AddAnimationStep(character, "Start");
			}
			else
			{
				character = new Character();
				AddAnimationStep(character, "Start");
			}

			try
			{
				// Execute program with animation tracking
				ExecuteCommandsWithAnimation(program.Commands, character, result.Trace);

				// Check if exercise is completed
				if (exercise != null)
				{
					if (exercise.IsCompleted(character.Position))
					{
						result.Status = ExecutionStatus.Success;
						LoggingService.Instance.LogInfo($"Animated exercise completed successfully!");
					}
					else
					{
						result.Status = ExecutionStatus.Failure;
						result.ErrorMessage = $"Character did not reach the end position. " +
											$"Current: {character.Position}, Target: {exercise.Grid.EndPosition}";
						LoggingService.Instance.LogWarning($"Animated exercise failed: {result.ErrorMessage}");
					}
				}
				else
				{
					result.Status = ExecutionStatus.Success;
					LoggingService.Instance.LogInfo("Animated program executed successfully (no exercise)");
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
				LoggingService.Instance.LogError($"OutOfBoundsException during animated execution: {ex.Message}");
			}
			catch (BlockedCellException ex)
			{
				result.Status = ExecutionStatus.RuntimeError;
				result.ErrorMessage = $"Blocked cell error: {ex.Message}";
				result.Exception = ex;
				result.FinalPosition = character.Position.Copy();
				result.FinalDirection = character.Direction;
				LoggingService.Instance.LogError($"BlockedCellException during animated execution: {ex.Message}");
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
				LoggingService.Instance.LogException(ex, "Unexpected exception during animated program execution");
			}

			LoggingService.Instance.LogInfo($"Animated program execution completed with {animationSteps.Count} steps");
			return result;
		}

		private void ExecuteCommandsWithAnimation(List<ICommand> commands, Character character, List<string> trace)
		{
			foreach (var command in commands)
			{
				if (command is RepeatCommand repeatCmd)
				{
					for (int i = 0; i < repeatCmd.Times; i++)
					{
						ExecuteCommandsWithAnimation(repeatCmd.Commands, character, trace);
					}
				}
				else if (command is RepeatUntilCommand repeatUntilCmd)
				{
					int iterations = 0;
					const int maxIterations = 10000;

					while (!EvaluateCondition(repeatUntilCmd.Condition, character) && iterations < maxIterations)
					{
						ExecuteCommandsWithAnimation(repeatUntilCmd.Commands, character, trace);
						iterations++;
					}

					if (iterations >= maxIterations)
					{
						throw new InvalidOperationException("RepeatUntil exceeded maximum iterations");
					}
				}
				else if (command is MoveCommand moveCmd)
				{
					// Execute move step by step for animation
					for (int i = 0; i < moveCmd.Steps; i++)
					{
						character.Move(1);
						AddAnimationStep(character, $"Move (step {i + 1} of {moveCmd.Steps})");
					}
					trace.Add($"Move {moveCmd.Steps}");
				}
				else if (command is TurnCommand turnCmd)
				{
					command.Execute(character);
					string dir = turnCmd.TurnDirection == TurnDirection.Left ? "left" : "right";
					AddAnimationStep(character, $"Turn {dir}");
					trace.Add($"Turn {dir}");
				}
				else
				{
					// Generic command execution
					command.Execute(character);
					AddAnimationStep(character, command.ToString());
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

		private void AddAnimationStep(Character character, string commandDescription)
		{
			var step = new AnimationStep(
				character.Position,
				character.Direction,
				commandDescription,
				stepCounter++
			);
			animationSteps.Add(step);
		}

		public List<AnimationStep> GetAnimationSteps()
		{
			return new List<AnimationStep>(animationSteps);
		}

		// Protected properties to access from base class
		protected Program program => GetProgram();
		protected PathfindingExercise exercise => GetExercise();

		// Helper methods to access private fields from base class
		private Program GetProgram()
		{
			var field = typeof(ProgramRunner).GetField("program", 
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			return field?.GetValue(this) as Program;
		}

		private PathfindingExercise GetExercise()
		{
			var field = typeof(ProgramRunner).GetField("exercise", 
				System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			return field?.GetValue(this) as PathfindingExercise;
		}
	}
}
