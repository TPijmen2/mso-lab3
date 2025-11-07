using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProgrammingLearningApp.UI.Controls
{
	public partial class CommandBlockControl : UserControl
	{
		public string CommandType { get; private set; }
		public int? Parameter { get; private set; }
		public string BlockColor { get; private set; }
		public bool IsContainer { get; private set; }

		public event EventHandler? DeleteRequested;
		public event EventHandler? ParameterChanged;

		private List<CommandBlockControl> childBlocks = new List<CommandBlockControl>();

		public CommandBlockControl(string commandType, int? parameter, string color, bool isContainer = false)
		{
			InitializeComponent();

			CommandType = commandType;
			Parameter = parameter;
			BlockColor = color;
			IsContainer = isContainer;

			InitializeBlock();
		}

		private void InitializeBlock()
		{
			// Set block color
			var brush = (SolidColorBrush?)new BrushConverter().ConvertFromString(BlockColor);
			if (brush != null)
			{
				BlockBorder.Background = brush;
			}

			// Set command label
			switch (CommandType)
			{
				case "Move":
					CommandLabel.Text = "Move";
					ParameterInput.Text = Parameter?.ToString() ?? "1";
					ParameterInput.Visibility = Visibility.Visible;
					break;

				case "TurnLeft":
					CommandLabel.Text = "Turn Left";
					break;

				case "TurnRight":
					CommandLabel.Text = "Turn Right";
					break;

				case "Repeat":
					CommandLabel.Text = "Repeat";
					ParameterInput.Text = Parameter?.ToString() ?? "2";
					ParameterInput.Visibility = Visibility.Visible;
					break;

				case "RepeatUntil":
					CommandLabel.Text = "Repeat Until Wall Ahead";
					break;
			}

			// Show container for nested blocks
			if (IsContainer)
			{
				ChildContainer.Visibility = Visibility.Visible;
			}
		}

		private void ParameterInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (int.TryParse(ParameterInput.Text, out int value))
			{
				Parameter = value;
				OnParameterChanged();
			}
		}

		private void Delete_Click(object sender, RoutedEventArgs e)
		{
			DeleteRequested?.Invoke(this, EventArgs.Empty);
		}

		private void ChildContainer_DragOver(object sender, DragEventArgs e)
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

		private void ChildContainer_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent("BlockData"))
			{
				var block = e.Data.GetData("BlockData") as CommandBlockControl;
				if (block != null)
				{
					AddChildBlock(block);
				}
			}
			e.Handled = true;
		}

		public void AddChildBlock(CommandBlockControl block)
		{
			EmptyChildMessage.Visibility = Visibility.Collapsed;

			// Create new instance
			var newBlock = new CommandBlockControl(
				block.CommandType,
				block.Parameter,
				block.BlockColor,
				block.IsContainer
			);

			newBlock.DeleteRequested += ChildBlock_DeleteRequested;
			newBlock.ParameterChanged += ChildBlock_ParameterChanged;

			// If the incoming block has children, recursively add them to the new block
			if (block.IsContainer)
			{
				var existingChildren = block.GetChildBlocks();
				foreach (var childBlock in existingChildren)
				{
					newBlock.AddChildBlock(childBlock);
				}
			}

			childBlocks.Add(newBlock);
			ChildPanel.Children.Add(newBlock);

			OnParameterChanged();
		}

		private void ChildBlock_DeleteRequested(object? sender, EventArgs e)
		{
			var block = sender as CommandBlockControl;
			if (block != null)
			{
				childBlocks.Remove(block);
				ChildPanel.Children.Remove(block);

				if (childBlocks.Count == 0)
				{
					EmptyChildMessage.Visibility = Visibility.Visible;
				}

				OnParameterChanged();
			}
		}

		private void ChildBlock_ParameterChanged(object? sender, EventArgs e)
		{
			OnParameterChanged();
		}

		public List<CommandBlockControl> GetChildBlocks()
		{
			return childBlocks;
		}

		protected virtual void OnParameterChanged()
		{
			ParameterChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
