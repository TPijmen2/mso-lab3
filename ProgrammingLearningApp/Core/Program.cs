using ProgrammingLearningApp.Models;

namespace ProgrammingLearningApp
{
	public class Program
	{
		public string Name { get; }
		public List<ICommand> Commands { get; }

		public Program(string name, List<ICommand> commands)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Program name cannot be empty");
			if (commands == null)
				throw new ArgumentException("Commands cannot be null");

			Name = name;
			Commands = commands;
		}

		public ExecutionResult Execute()
		{
			var result = new ExecutionResult();
			var character = new Character();

			ExecuteCommands(Commands, character, result.Trace);

			result.FinalPosition = character.Position.Copy();
			result.FinalDirection = character.Direction;

			return result;
		}

		private void ExecuteCommands(List<ICommand> commands, Character character, List<string> trace)
		{
			foreach (var command in commands)
			{
				if (command is RepeatCommand repeatCmd)
				{
					for (int i = 0; i < repeatCmd.Times; i++)
					{
						ExecuteCommands(repeatCmd.Commands, character, trace);
					}
				}
				else
				{
					command.Execute(character);

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

		public ProgramMetrics CalculateMetrics()
		{
			return new ProgramMetrics
			{
				CommandCount = Commands.Sum(c => c.GetCommandCount()),
				MaxNestingLevel = Commands.Count > 0 ? Commands.Max(c => c.GetMaxNestingLevel()) : 0,
				RepeatCount = Commands.Sum(c => c.GetRepeatCount())
			};
		}

		public string GetTextRepresentation()
		{
			return string.Join("\n", Commands.Select(c => c.ToString(0)));
		}
	}
}