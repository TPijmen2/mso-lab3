using System;

namespace ProgrammingLearningApp
{
	public class MoveCommand : ICommand
	{
		public int Steps { get; }

		public MoveCommand(int steps)
		{
			if (steps < 0)
				throw new ArgumentException("Steps must be non-negative");
			Steps = steps;
		}

		public void Execute(Character character)
		{
			character.Move(Steps);
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
			return new string(' ', indentLevel * 4) + $"Move {Steps}";
		}
	}
}