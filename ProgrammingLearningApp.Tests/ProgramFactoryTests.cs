using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class ProgramFactoryTests
	{
		[Fact]
		public void CreateBasicProgram_CreatesValidProgram()
		{
			// Act
			var program = ProgramFactory.CreateBasicProgram();

			// Assert
			Assert.NotNull(program);
			Assert.Equal("Rectangle1", program.Name);
			Assert.Equal(8, program.Commands.Count);
		}

		[Fact]
		public void CreateAdvancedProgram_CreatesValidProgram()
		{
			// Act
			var program = ProgramFactory.CreateAdvancedProgram();

			// Assert
			Assert.NotNull(program);
			Assert.Equal("Rectangle2", program.Name);
			Assert.Single(program.Commands);
			Assert.IsType<RepeatCommand>(program.Commands[0]);
		}

		[Fact]
		public void CreateExpertProgram_CreatesValidProgram()
		{
			// Act
			var program = ProgramFactory.CreateExpertProgram();

			// Assert
			Assert.NotNull(program);
			Assert.Equal("Random", program.Name);
			Assert.True(program.Commands.Count > 0);
		}
	}
}