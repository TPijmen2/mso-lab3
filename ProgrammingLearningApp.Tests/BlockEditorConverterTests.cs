using ProgrammingLearningApp.Commands;
using ProgrammingLearningApp.Models;
using ProgrammingLearningApp.Services;
using Xunit;

namespace ProgrammingLearningApp.Tests
{

	public class BlockEditorConverterTests
	{
		private readonly BlockCommandConverter _converter;

		public BlockEditorConverterTests()
		{
			_converter = new BlockCommandConverter();
		}

		[Fact]
		public void ConvertCommandToBlock_MoveCommand_CreatesCorrectBlock()
		{
			// Arrange
			var command = new MoveCommand(5);

			// Act
			var block = _converter.ConvertCommandToBlock(command);

			// Assert
			Assert.NotNull(block);
			Assert.Equal("Move", block.CommandType);
			Assert.Equal(5, block.Parameter);
			Assert.Equal("#4CAF50", block.BlockColor);
			Assert.False(block.IsContainer);
		}

		[Fact]
		public void ConvertCommandToBlock_TurnLeftCommand_CreatesCorrectBlock()
		{
			// Arrange
			var command = new TurnCommand(TurnDirection.Left);

			// Act
			var block = _converter.ConvertCommandToBlock(command);

			// Assert
			Assert.NotNull(block);
			Assert.Equal("TurnLeft", block.CommandType);
			Assert.Null(block.Parameter);
			Assert.Equal("#2196F3", block.BlockColor);
			Assert.False(block.IsContainer);
		}

		[Fact]
		public void ConvertCommandToBlock_TurnRightCommand_CreatesCorrectBlock()
		{
			// Arrange
			var command = new TurnCommand(TurnDirection.Right);

			// Act
			var block = _converter.ConvertCommandToBlock(command);

			// Assert
			Assert.NotNull(block);
			Assert.Equal("TurnRight", block.CommandType);
			Assert.Null(block.Parameter);
			Assert.Equal("#FF9800", block.BlockColor);
			Assert.False(block.IsContainer);
		}

		[Fact]
		public void ConvertCommandToBlock_RepeatCommand_CreatesCorrectBlockWithChildren()
		{
			// Arrange
			var childCommands = new List<ICommand>
			{
				new MoveCommand(3),
				new TurnCommand(TurnDirection.Right)
			};
			var command = new RepeatCommand(4, childCommands);

			// Act
			var block = _converter.ConvertCommandToBlock(command);

			// Assert
			Assert.NotNull(block);
			Assert.Equal("Repeat", block.CommandType);
			Assert.Equal(4, block.Parameter);
			Assert.Equal("#9C27B0", block.BlockColor);
			Assert.True(block.IsContainer);
			Assert.Equal(2, block.Children.Count);
			
			Assert.Equal("Move", block.Children[0].CommandType);
			Assert.Equal(3, block.Children[0].Parameter);
			
			Assert.Equal("TurnRight", block.Children[1].CommandType);
		}

		[Fact]
		public void ConvertCommandToBlock_NestedRepeat_PreservesAllLevels()
		{
			// Arrange
			var innerCommands = new List<ICommand>
			{
				new MoveCommand(1)
			};
			var innerRepeat = new RepeatCommand(2, innerCommands);
			
			var outerCommands = new List<ICommand>
			{
				new MoveCommand(5),
				innerRepeat
			};
			var outerRepeat = new RepeatCommand(3, outerCommands);

			// Act
			var block = _converter.ConvertCommandToBlock(outerRepeat);

			// Assert
			Assert.NotNull(block);
			Assert.Equal("Repeat", block.CommandType);
			Assert.Equal(3, block.Parameter);
			Assert.Equal(2, block.Children.Count);
			
			// Check nested repeat
			var nestedBlock = block.Children[1];
			Assert.Equal("Repeat", nestedBlock.CommandType);
			Assert.Equal(2, nestedBlock.Parameter);
			Assert.Single(nestedBlock.Children);
			Assert.Equal("Move", nestedBlock.Children[0].CommandType);
		}

		[Fact]
		public void ConvertCommandToBlock_RepeatUntilCommand_CreatesCorrectBlock()
		{
			// Arrange
			var childCommands = new List<ICommand>
			{
				new MoveCommand(1),
				new TurnCommand(TurnDirection.Left)
			};
			var command = new RepeatUntilCommand(Condition.WallAhead, childCommands);

			// Act
			var block = _converter.ConvertCommandToBlock(command);

			// Assert
			Assert.NotNull(block);
			Assert.Equal("RepeatUntil", block.CommandType);
			Assert.Null(block.Parameter);
			Assert.Equal("#673AB7", block.BlockColor);
			Assert.True(block.IsContainer);
			Assert.Equal(2, block.Children.Count);
		}

		[Fact]
		public void ConvertBlockToCommand_MoveBlock_CreatesCorrectCommand()
		{
			// Arrange
			var block = new BlockRepresentation("Move", 5, "#4CAF50", false);

			// Act
			var command = _converter.ConvertBlockToCommand(block);

			// Assert
			Assert.NotNull(command);
			var moveCommand = Assert.IsType<MoveCommand>(command);
			Assert.Equal(5, moveCommand.Steps);
		}

		[Fact]
		public void ConvertBlockToCommand_TurnLeftBlock_CreatesCorrectCommand()
		{
			// Arrange
			var block = new BlockRepresentation("TurnLeft", null, "#2196F3", false);

			// Act
			var command = _converter.ConvertBlockToCommand(block);

			// Assert
			Assert.NotNull(command);
			var turnCommand = Assert.IsType<TurnCommand>(command);
			Assert.Equal(TurnDirection.Left, turnCommand.TurnDirection);
		}

		[Fact]
		public void ConvertBlockToCommand_TurnRightBlock_CreatesCorrectCommand()
		{
			// Arrange
			var block = new BlockRepresentation("TurnRight", null, "#FF9800", false);

			// Act
			var command = _converter.ConvertBlockToCommand(block);

			// Assert
			Assert.NotNull(command);
			var turnCommand = Assert.IsType<TurnCommand>(command);
			Assert.Equal(TurnDirection.Right, turnCommand.TurnDirection);
		}

		[Fact]
		public void ConvertBlockToCommand_RepeatBlock_CreatesCorrectCommand()
		{
			// Arrange
			var block = new BlockRepresentation("Repeat", 4, "#9C27B0", true);
			block.AddChild(new BlockRepresentation("Move", 3, "#4CAF50", false));
			block.AddChild(new BlockRepresentation("TurnRight", null, "#FF9800", false));

			// Act
			var command = _converter.ConvertBlockToCommand(block);

			// Assert
			Assert.NotNull(command);
			var repeatCommand = Assert.IsType<RepeatCommand>(command);
			Assert.Equal(4, repeatCommand.Times);
			Assert.Equal(2, repeatCommand.Commands.Count);
			
			Assert.IsType<MoveCommand>(repeatCommand.Commands[0]);
			Assert.Equal(3, ((MoveCommand)repeatCommand.Commands[0]).Steps);
			
			Assert.IsType<TurnCommand>(repeatCommand.Commands[1]);
			Assert.Equal(TurnDirection.Right, ((TurnCommand)repeatCommand.Commands[1]).TurnDirection);
		}

		[Fact]
		public void ConvertBlockToCommand_NestedRepeatBlock_PreservesStructure()
		{
			// Arrange
			var innerBlock = new BlockRepresentation("Repeat", 2, "#9C27B0", true);
			innerBlock.AddChild(new BlockRepresentation("Move", 1, "#4CAF50", false));
			
			var outerBlock = new BlockRepresentation("Repeat", 3, "#9C27B0", true);
			outerBlock.AddChild(new BlockRepresentation("Move", 5, "#4CAF50", false));
			outerBlock.AddChild(innerBlock);

			// Act
			var command = _converter.ConvertBlockToCommand(outerBlock);

			// Assert
			Assert.NotNull(command);
			var outerRepeat = Assert.IsType<RepeatCommand>(command);
			Assert.Equal(3, outerRepeat.Times);
			Assert.Equal(2, outerRepeat.Commands.Count);
			
			var innerRepeat = Assert.IsType<RepeatCommand>(outerRepeat.Commands[1]);
			Assert.Equal(2, innerRepeat.Times);
			Assert.Single(innerRepeat.Commands);
		}

		[Fact]
		public void ConvertBlockToCommand_RepeatUntilBlock_CreatesCorrectCommand()
		{
			// Arrange
			var block = new BlockRepresentation("RepeatUntil", null, "#673AB7", true);
			block.AddChild(new BlockRepresentation("Move", 1, "#4CAF50", false));

			// Act
			var command = _converter.ConvertBlockToCommand(block);

			// Assert
			Assert.NotNull(command);
			var repeatUntilCommand = Assert.IsType<RepeatUntilCommand>(command);
			Assert.Equal(Condition.WallAhead, repeatUntilCommand.Condition);
			Assert.Single(repeatUntilCommand.Commands);
		}

		// ===== Program Conversion Tests =====

		[Fact]
		public void ConvertProgramToBlocks_SimpleProgram_CreatesCorrectBlocks()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(10),
				new TurnCommand(TurnDirection.Right),
				new MoveCommand(5)
			};
			var program = new Program("Test", commands);

			// Act
			var blocks = _converter.ConvertProgramToBlocks(program);

			// Assert
			Assert.Equal(3, blocks.Count);
			Assert.Equal("Move", blocks[0].CommandType);
			Assert.Equal(10, blocks[0].Parameter);
			Assert.Equal("TurnRight", blocks[1].CommandType);
			Assert.Equal("Move", blocks[2].CommandType);
			Assert.Equal(5, blocks[2].Parameter);
		}

		[Fact]
		public void ConvertProgramToBlocks_NullProgram_ReturnsEmptyList()
		{
			// Act
			var blocks = _converter.ConvertProgramToBlocks(null);

			// Assert
			Assert.Empty(blocks);
		}

		[Fact]
		public void ConvertBlocksToProgram_SimpleBlocks_CreatesCorrectProgram()
		{
			// Arrange
			var blocks = new List<BlockRepresentation>
			{
				new BlockRepresentation("Move", 10, "#4CAF50", false),
				new BlockRepresentation("TurnRight", null, "#FF9800", false),
				new BlockRepresentation("Move", 5, "#4CAF50", false)
			};

			// Act
			var program = _converter.ConvertBlocksToProgram(blocks);

			// Assert
			Assert.NotNull(program);
			Assert.Equal("Block Program", program.Name);
			Assert.Equal(3, program.Commands.Count);
			
			Assert.IsType<MoveCommand>(program.Commands[0]);
			Assert.Equal(10, ((MoveCommand)program.Commands[0]).Steps);
			
			Assert.IsType<TurnCommand>(program.Commands[1]);
			Assert.Equal(TurnDirection.Right, ((TurnCommand)program.Commands[1]).TurnDirection);
			
			Assert.IsType<MoveCommand>(program.Commands[2]);
			Assert.Equal(5, ((MoveCommand)program.Commands[2]).Steps);
		}

		[Fact]
		public void ConvertBlocksToProgram_EmptyBlocks_CreatesEmptyProgram()
		{
			// Arrange
			var blocks = new List<BlockRepresentation>();

			// Act
			var program = _converter.ConvertBlocksToProgram(blocks);

			// Assert
			Assert.NotNull(program);
			Assert.Empty(program.Commands);
		}

		[Fact]
		public void RoundTrip_SimpleCommands_PreservesStructure()
		{
			// Arrange
			var originalCommands = new List<ICommand>
			{
				new MoveCommand(10),
				new TurnCommand(TurnDirection.Right),
				new MoveCommand(5)
			};
			var originalProgram = new Program("Test", originalCommands);

			// Act - Convert to blocks and back
			var blocks = _converter.ConvertProgramToBlocks(originalProgram);
			var resultProgram = _converter.ConvertBlocksToProgram(blocks);

			// Assert
			Assert.Equal(3, resultProgram.Commands.Count);
			Assert.IsType<MoveCommand>(resultProgram.Commands[0]);
			Assert.Equal(10, ((MoveCommand)resultProgram.Commands[0]).Steps);
			
			Assert.IsType<TurnCommand>(resultProgram.Commands[1]);
			Assert.Equal(TurnDirection.Right, ((TurnCommand)resultProgram.Commands[1]).TurnDirection);
			
			Assert.IsType<MoveCommand>(resultProgram.Commands[2]);
			Assert.Equal(5, ((MoveCommand)resultProgram.Commands[2]).Steps);
		}

		[Fact]
		public void RoundTrip_ComplexNestedProgram_PreservesStructure()
		{
			// Arrange
			var innerCommands = new List<ICommand>
			{
				new MoveCommand(2),
				new TurnCommand(TurnDirection.Left)
			};
			var innerRepeat = new RepeatCommand(3, innerCommands);
			
			var outerCommands = new List<ICommand>
			{
				new MoveCommand(5),
				innerRepeat,
				new TurnCommand(TurnDirection.Right)
			};
			var outerRepeat = new RepeatCommand(2, outerCommands);
			
			var originalProgram = new Program("Complex", new List<ICommand> { outerRepeat });

			// Act
			var blocks = _converter.ConvertProgramToBlocks(originalProgram);
			var resultProgram = _converter.ConvertBlocksToProgram(blocks);

			// Assert
			Assert.Single(resultProgram.Commands);
			var resultOuter = Assert.IsType<RepeatCommand>(resultProgram.Commands[0]);
			Assert.Equal(2, resultOuter.Times);
			Assert.Equal(3, resultOuter.Commands.Count);
			
			var resultInner = Assert.IsType<RepeatCommand>(resultOuter.Commands[1]);
			Assert.Equal(3, resultInner.Times);
			Assert.Equal(2, resultInner.Commands.Count);
		}

		[Fact]
		public void RoundTrip_ProgramMetrics_ArePreserved()
		{
			// Arrange
			var innerCommands = new List<ICommand>
			{
				new MoveCommand(1),
				new TurnCommand(TurnDirection.Right)
			};
			var repeat = new RepeatCommand(3, innerCommands);
			var originalProgram = new Program("Metrics Test", new List<ICommand> { repeat });
			var originalMetrics = originalProgram.CalculateMetrics();

			// Act
			var blocks = _converter.ConvertProgramToBlocks(originalProgram);
			var resultProgram = _converter.ConvertBlocksToProgram(blocks);
			var resultMetrics = resultProgram.CalculateMetrics();

			// Assert
			Assert.Equal(originalMetrics.CommandCount, resultMetrics.CommandCount);
			Assert.Equal(originalMetrics.MaxNestingLevel, resultMetrics.MaxNestingLevel);
			Assert.Equal(originalMetrics.RepeatCount, resultMetrics.RepeatCount);
		}

		[Fact]
		public void RoundTrip_TextRepresentation_IsConsistent()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(5),
				new TurnCommand(TurnDirection.Right),
				new MoveCommand(3)
			};
			var originalProgram = new Program("Text Test", commands);
			var originalText = originalProgram.GetTextRepresentation();

			// Act
			var blocks = _converter.ConvertProgramToBlocks(originalProgram);
			var resultProgram = _converter.ConvertBlocksToProgram(blocks, "Text Test");
			var resultText = resultProgram.GetTextRepresentation();

			// Assert
			Assert.Equal(originalText, resultText);
		}

		[Fact]
		public void RoundTrip_RepeatUntilWithCondition_PreservesCondition()
		{
			// Arrange
			var commands = new List<ICommand>
			{
				new MoveCommand(1)
			};
			var repeatUntil = new RepeatUntilCommand(Condition.WallAhead, commands);
			var originalProgram = new Program("Condition Test", new List<ICommand> { repeatUntil });

			// Act
			var blocks = _converter.ConvertProgramToBlocks(originalProgram);
			var resultProgram = _converter.ConvertBlocksToProgram(blocks);

			// Assert
			Assert.Single(resultProgram.Commands);
			var resultRepeatUntil = Assert.IsType<RepeatUntilCommand>(resultProgram.Commands[0]);
			Assert.Equal(Condition.WallAhead, resultRepeatUntil.Condition);
		}
	}
}
