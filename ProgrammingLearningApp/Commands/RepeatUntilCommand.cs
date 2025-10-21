using ProgrammingLearningApp.Models;
using System.Text;

namespace ProgrammingLearningApp.Commands
{
	public class RepeatUntilCommand : ICommand
	{
		public Condition Condition { get; }
		public List<ICommand> Commands { get; }
		private const int MaxIterations = 10000; // Safety limit

		public RepeatUntilCommand(Condition condition, List<ICommand> commands)
		{
			if (commands == null || commands.Count == 0)
				throw new ArgumentException("Commands list cannot be empty");

			Condition = condition;
			Commands = commands;
		}

		public void Execute(Character character)
		{
			int iterations = 0;
			while (!EvaluateCondition(character) && iterations < MaxIterations)
			{
				foreach (var command in Commands)
				{
					command.Execute(character);
				}
				iterations++;
			}

			if (iterations >= MaxIterations)
				throw new InvalidOperationException("RepeatUntil exceeded maximum iterations - possible infinite loop");
		}

		private bool EvaluateCondition(Character character)
		{
			if (character.Grid == null)
				return false; // No grid loaded, conditions are always false

			switch (Condition)
			{
				case Condition.WallAhead:
					return IsWallAhead(character);
				case Condition.GridEdge:
					return IsAtGridEdge(character);
				default:
					throw new NotImplementedException($"Condition {Condition} not implemented");
			}
		}

		private bool IsWallAhead(Character character)
		{
			var nextPos = character.Grid.GetNextPosition(character.Position, character.Direction);

			// Check if next position is out of bounds or blocked
			if (!character.Grid.IsWithinBounds(nextPos))
				return true;

			return character.Grid.IsCellBlocked(nextPos);
		}

		private bool IsAtGridEdge(Character character)
		{
			var nextPos = character.Grid.GetNextPosition(character.Position, character.Direction);
			return !character.Grid.IsWithinBounds(nextPos);
		}

		public int GetCommandCount()
		{
			return 1 + Commands.Sum(c => c.GetCommandCount());
		}

		public int GetMaxNestingLevel()
		{
			int maxChildNesting = Commands.Count > 0
				? Commands.Max(c => c.GetMaxNestingLevel())
				: 0;
			return 1 + maxChildNesting;
		}

		public int GetRepeatCount()
		{
			return 1 + Commands.Sum(c => c.GetRepeatCount());
		}

		public string ToString(int indentLevel = 0)
		{
			string indent = new string(' ', indentLevel * 4);
			string conditionStr = Condition == Condition.WallAhead ? "WallAhead" : "GridEdge";
			StringBuilder result = new StringBuilder();
			result.AppendLine(indent + $"RepeatUntil {conditionStr}");
			foreach (var command in Commands)
			{
				result.AppendLine(command.ToString(indentLevel + 1));
			}
			return result.ToString().TrimEnd('\n', '\r');
		}
	}
}