using System.Collections.Generic;

namespace ProgrammingLearningApp
{
	public static class ProgramFactory
	{
		public static Program CreateBasicProgram()
		{
			// Rectangle1: Simple moves and turns, no repeats
			var commands = new List<ICommand>
			{
				new MoveCommand(10),
				new TurnCommand(TurnDirection.Right),
				new MoveCommand(10),
				new TurnCommand(TurnDirection.Right),
				new MoveCommand(10),
				new TurnCommand(TurnDirection.Right),
				new MoveCommand(10),
				new TurnCommand(TurnDirection.Right)
			};
			return new Program("Rectangle1", commands);
		}

		public static Program CreateAdvancedProgram()
		{
			// Rectangle2: Using one repeat statement
			var repeatCommands = new List<ICommand>
			{
				new MoveCommand(10),
				new TurnCommand(TurnDirection.Right)
			};

			var commands = new List<ICommand>
			{
				new RepeatCommand(4, repeatCommands)
			};

			return new Program("Rectangle2", commands);
		}

		public static Program CreateExpertProgram()
		{
			// Random: Nested repeats
			var innerRepeat1 = new List<ICommand>
			{
				new MoveCommand(1),
				new TurnCommand(TurnDirection.Right)
			};

			var innerRepeat2 = new List<ICommand>
			{
				new MoveCommand(2),
				new TurnCommand(TurnDirection.Left)
			};

			var commands = new List<ICommand>
			{
				new MoveCommand(5),
				new TurnCommand(TurnDirection.Left),
				new TurnCommand(TurnDirection.Left),
				new MoveCommand(3),
				new TurnCommand(TurnDirection.Right),
				new RepeatCommand(3, innerRepeat1),
				new RepeatCommand(5, innerRepeat2)
			};

			return new Program("Random", commands);
		}
	}
}