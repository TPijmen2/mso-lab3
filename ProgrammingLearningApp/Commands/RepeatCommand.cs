using System;
using System.Collections.Generic;
using System.Linq;

namespace ProgrammingLearningApp
{
	public class RepeatCommand : ICommand
	{
		public int Times { get; }
		public List<ICommand> Commands { get; }

		public RepeatCommand(int times, List<ICommand> commands)
		{
			if (times < 0)
				throw new ArgumentException("Times must be non-negative");
			if (commands == null || commands.Count == 0)
				throw new ArgumentException("Commands list cannot be empty");

			Times = times;
			Commands = commands;
		}

		public void Execute(Character character)
		{
			for (int i = 0; i < Times; i++)
			{
				foreach (var command in Commands)
				{
					command.Execute(character);
				}
			}
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
			string result = indent + $"Repeat {Times} times\n";
			foreach (var command in Commands)
			{
				result += command.ToString(indentLevel + 1) + "\n";
			}
			return result.TrimEnd('\n');
		}
	}
}