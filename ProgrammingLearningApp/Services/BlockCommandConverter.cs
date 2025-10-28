using ProgrammingLearningApp.Commands;
using ProgrammingLearningApp.Models;
using System.Collections.Generic;

namespace ProgrammingLearningApp.Services
{
	public class BlockCommandConverter
	{
		public BlockRepresentation ConvertCommandToBlock(ICommand command)
		{
			if (command is MoveCommand moveCmd)
			{
				return new BlockRepresentation("Move", moveCmd.Steps, "#4CAF50", false);
			}
			else if (command is TurnCommand turnCmd)
			{
				if (turnCmd.TurnDirection == TurnDirection.Left)
				{
					return new BlockRepresentation("TurnLeft", null, "#2196F3", false);
				}
				else
				{
					return new BlockRepresentation("TurnRight", null, "#FF9800", false);
				}
			}
			else if (command is RepeatCommand repeatCmd)
			{
				var block = new BlockRepresentation("Repeat", repeatCmd.Times, "#9C27B0", true);
				foreach (var childCommand in repeatCmd.Commands)
				{
					var childBlock = ConvertCommandToBlock(childCommand);
					block.AddChild(childBlock);
				}
				return block;
			}
			else if (command is RepeatUntilCommand repeatUntilCmd)
			{
				var block = new BlockRepresentation("RepeatUntil", null, "#673AB7", true);
				foreach (var childCommand in repeatUntilCmd.Commands)
				{
					var childBlock = ConvertCommandToBlock(childCommand);
					block.AddChild(childBlock);
				}
				return block;
			}

			return null;
		}

		public ICommand ConvertBlockToCommand(BlockRepresentation block)
		{
			switch (block.CommandType)
			{
				case "Move":
					int steps = block.Parameter ?? 1;
					return new MoveCommand(steps);

				case "TurnLeft":
					return new TurnCommand(TurnDirection.Left);

				case "TurnRight":
					return new TurnCommand(TurnDirection.Right);

				case "Repeat":
					int times = block.Parameter ?? 2;
					var repeatCommands = new List<ICommand>();
					foreach (var childBlock in block.Children)
					{
						var childCommand = ConvertBlockToCommand(childBlock);
						if (childCommand != null)
						{
							repeatCommands.Add(childCommand);
						}
					}
					if (repeatCommands.Count > 0)
					{
						return new RepeatCommand(times, repeatCommands);
					}
					break;

				case "RepeatUntil":
					var repeatUntilCommands = new List<ICommand>();
					foreach (var childBlock in block.Children)
					{
						var childCommand = ConvertBlockToCommand(childBlock);
						if (childCommand != null)
						{
							repeatUntilCommands.Add(childCommand);
						}
					}
					if (repeatUntilCommands.Count > 0)
					{
						return new RepeatUntilCommand(Models.Condition.WallAhead, repeatUntilCommands);
					}
					break;
			}

			return null;
		}

		public List<BlockRepresentation> ConvertProgramToBlocks(Program program)
		{
			var blocks = new List<BlockRepresentation>();
			if (program?.Commands != null)
			{
				foreach (var command in program.Commands)
				{
					var block = ConvertCommandToBlock(command);
					if (block != null)
					{
						blocks.Add(block);
					}
				}
			}
			return blocks;
		}

		public Program ConvertBlocksToProgram(List<BlockRepresentation> blocks, string programName = "Block Program")
		{
			var commands = new List<ICommand>();
			foreach (var block in blocks)
			{
				var command = ConvertBlockToCommand(block);
				if (command != null)
				{
					commands.Add(command);
				}
			}
			return new Program(programName, commands);
		}
	}

	public class BlockRepresentation
	{
		public string CommandType { get; set; }
		public int? Parameter { get; set; }
		public string BlockColor { get; set; }
		public bool IsContainer { get; set; }
		public List<BlockRepresentation> Children { get; set; }

		public BlockRepresentation(string commandType, int? parameter, string color, bool isContainer)
		{
			CommandType = commandType;
			Parameter = parameter;
			BlockColor = color;
			IsContainer = isContainer;
			Children = new List<BlockRepresentation>();
		}

		public void AddChild(BlockRepresentation child)
		{
			Children.Add(child);
		}
	}
}
