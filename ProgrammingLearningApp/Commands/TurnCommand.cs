using ProgrammingLearningApp.Models;

namespace ProgrammingLearningApp
{
	public enum TurnDirection
	{
		Left,
		Right
	}

	public class TurnCommand : ICommand
	{
		public TurnDirection TurnDirection { get; }

		public TurnCommand(TurnDirection direction)
		{
			TurnDirection = direction;
		}

		public void Execute(Character character)
		{
			if (TurnDirection == TurnDirection.Left)
				character.TurnLeft();
			else
				character.TurnRight();
		}

		public int GetCommandCount()
		{
			return 1;
		}

		public int GetMaxNestingLevel()
		{
			return 0;
		}

		public int GetRepeatCount()
		{
			return 0;
		}

		public string ToString(int indentLevel = 0)
		{
			string direction = TurnDirection == TurnDirection.Left ? "left" : "right";
			return new string(' ', indentLevel * 4) + $"Turn {direction}";
		}
	}
}