using ProgrammingLearningApp.Models;
using ProgrammingLearningApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ProgrammingLearningApp.Commands;

namespace ProgrammingLearningApp.UI.Controls
{
	public partial class BlockEditor : UserControl
	{
		private List<CommandBlockControl> commandBlocks = new List<CommandBlockControl>();
		private Point dragStartPoint;
		private bool isDragging = false;
		
		public event EventHandler? ProgramChanged;

		public BlockEditor()
		{
			InitializeComponent();
		}

		// Palette block mouse down handlers - track start of drag
		private void MoveBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			dragStartPoint = e.GetPosition(null);
			isDragging = false;
		}

		private void TurnLeftBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			dragStartPoint = e.GetPosition(null);
			isDragging = false;
		}

		private void TurnRightBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			dragStartPoint = e.GetPosition(null);
			isDragging = false;
		}

		private void RepeatBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			dragStartPoint = e.GetPosition(null);
			isDragging = false;
		}

		private void RepeatUntilBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			dragStartPoint = e.GetPosition(null);
			isDragging = false;
		}

		private void PaletteBlock_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !isDragging)
			{
				Point currentPosition = e.GetPosition(null);
				Vector diff = dragStartPoint - currentPosition;

				// Check if mouse moved enough to start drag
				if (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
					Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
				{
					isDragging = true;
					
					// Get the block type from the sender
					Border border = sender as Border;
					if (border != null && border.Tag != null)
					{
						string blockType = border.Tag.ToString();
						StartDrag(border, blockType);
					}
				}
			}
		}

		private void StartDrag(UIElement source, string blockType)
		{
			// Create block data based on type
			CommandBlockControl block = null;
			
			switch (blockType)
			{
				case "Move":
					block = new CommandBlockControl("Move", 1, "#4CAF50");
					break;
				case "TurnLeft":
					block = new CommandBlockControl("TurnLeft", null, "#2196F3");
					break;
				case "TurnRight":
					block = new CommandBlockControl("TurnRight", null, "#FF9800");
					break;
				case "Repeat":
					block = new CommandBlockControl("Repeat", 2, "#9C27B0", true);
					break;
				case "RepeatUntil":
					block = new CommandBlockControl("RepeatUntil", null, "#673AB7", true);
					break;
			}

			if (block != null)
			{
				DataObject data = new DataObject("BlockData", block);
				DragDrop.DoDragDrop(source, data, DragDropEffects.Copy);
			}
		}

		private void WorkArea_DragOver(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("BlockData"))
			{
				e.Effects = DragDropEffects.Copy;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
			e.Handled = true;
		}

		private void WorkArea_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("BlockData"))
			{
				var block = e.Data.GetData("BlockData") as CommandBlockControl;
				if (block != null)
				{
					AddBlockToWorkArea(block);
				}
			}
			e.Handled = true;
		}

		private void AddBlockToWorkArea(CommandBlockControl block)
		{
			EmptyMessage.Visibility = Visibility.Collapsed;

			// Create new instance to add to work area
			var newBlock = new CommandBlockControl(
				block.CommandType,
				block.Parameter,
				block.BlockColor,
				block.IsContainer
			);

			newBlock.DeleteRequested += Block_DeleteRequested;
			newBlock.ParameterChanged += Block_ParameterChanged;

			// If the block is a container and has children, copy them to the new block
			if (block.IsContainer)
			{
				var existingChildren = block.GetChildBlocks();
				foreach (var childBlock in existingChildren)
				{
					newBlock.AddChildBlock(childBlock);
				}
			}

			commandBlocks.Add(newBlock);
			WorkAreaPanel.Children.Add(newBlock);

			OnProgramChanged();
		}

		private void Block_DeleteRequested(object? sender, EventArgs e)
		{
			var block = sender as CommandBlockControl;
			if (block != null)
			{
				commandBlocks.Remove(block);
				WorkAreaPanel.Children.Remove(block);

				if (commandBlocks.Count == 0)
				{
					EmptyMessage.Visibility = Visibility.Visible;
				}

				OnProgramChanged();
			}
		}

		private void Block_ParameterChanged(object? sender, EventArgs e)
		{
			OnProgramChanged();
		}

		private void ClearAll_Click(object sender, RoutedEventArgs e)
		{
			commandBlocks.Clear();
			WorkAreaPanel.Children.Clear();
			WorkAreaPanel.Children.Add(EmptyMessage);
			EmptyMessage.Visibility = Visibility.Visible;
			OnProgramChanged();
		}

		public Program GetProgram()
		{
			var commands = new List<ICommand>();

			foreach (var block in commandBlocks)
			{
				var command = ConvertBlockToCommand(block);
				if (command != null)
				{
					commands.Add(command);
				}
			}

			LoggingService.Instance.LogDebug($"GetProgram: Created program with {commands.Count} commands");
			return new Program("Block Program", commands);
		}

		private ICommand? ConvertBlockToCommand(CommandBlockControl block)
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
					var childBlocks = block.GetChildBlocks();
					LoggingService.Instance.LogDebug($"Converting Repeat block with {childBlocks.Count} child blocks");
					foreach (var childBlock in childBlocks)
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
					var repeatUntilChildBlocks = block.GetChildBlocks();
					LoggingService.Instance.LogDebug($"Converting RepeatUntil block with {repeatUntilChildBlocks.Count} child blocks");
					foreach (var childBlock in repeatUntilChildBlocks)
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

		public void LoadProgram(Program? program)
		{
			LoggingService.Instance.LogInfo($"LoadProgram called with program: {program?.Name ?? "null"}");
			ClearAll_Click(null!, null!);

			if (program == null || program.Commands == null)
			{
				LoggingService.Instance.LogWarning("LoadProgram: Program or commands is null");
				return;
			}

			LoggingService.Instance.LogInfo($"LoadProgram: Loading {program.Commands.Count} commands");

			foreach (var command in program.Commands)
			{
				var block = ConvertCommandToBlock(command);
				if (block != null)
				{
					LoggingService.Instance.LogDebug($"LoadProgram: Adding block of type {block.CommandType}");
					AddBlockToWorkArea(block);
				}
			}
			
			LoggingService.Instance.LogInfo($"LoadProgram: Finished loading. Total blocks in work area: {commandBlocks.Count}");
		}

		private CommandBlockControl? ConvertCommandToBlock(ICommand command)
		{
			if (command is MoveCommand moveCmd)
			{
				return new CommandBlockControl("Move", moveCmd.Steps, "#4CAF50");
			}
			else if (command is TurnCommand turnCmd)
			{
				if (turnCmd.TurnDirection == TurnDirection.Left)
				{
					return new CommandBlockControl("TurnLeft", null, "#2196F3");
				}
				else
				{
					return new CommandBlockControl("TurnRight", null, "#FF9800");
				}
			}
			else if (command is RepeatCommand repeatCmd)
			{
				var block = new CommandBlockControl("Repeat", repeatCmd.Times, "#9C27B0", true);
				LoggingService.Instance.LogDebug($"ConvertCommandToBlock: Creating Repeat block with {repeatCmd.Commands.Count} child commands");
				foreach (var childCommand in repeatCmd.Commands)
				{
					var childBlock = ConvertCommandToBlock(childCommand);
					if (childBlock != null)
					{
						LoggingService.Instance.LogDebug($"ConvertCommandToBlock: Adding child block {childBlock.CommandType} to Repeat");
						block.AddChildBlock(childBlock);
					}
				}
				LoggingService.Instance.LogDebug($"ConvertCommandToBlock: Repeat block now has {block.GetChildBlocks().Count} child blocks");
				return block;
			}
			else if (command is RepeatUntilCommand repeatUntilCmd)
			{
				var block = new CommandBlockControl("RepeatUntil", null, "#673AB7", true);
				LoggingService.Instance.LogDebug($"ConvertCommandToBlock: Creating RepeatUntil block with {repeatUntilCmd.Commands.Count} child commands");
				foreach (var childCommand in repeatUntilCmd.Commands)
				{
					var childBlock = ConvertCommandToBlock(childCommand);
					if (childBlock != null)
					{
						LoggingService.Instance.LogDebug($"ConvertCommandToBlock: Adding child block {childBlock.CommandType} to RepeatUntil");
						block.AddChildBlock(childBlock);
					}
				}
				LoggingService.Instance.LogDebug($"ConvertCommandToBlock: RepeatUntil block now has {block.GetChildBlocks().Count} child blocks");
				return block;
			}

			return null;
		}

		protected virtual void OnProgramChanged()
		{
			ProgramChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
