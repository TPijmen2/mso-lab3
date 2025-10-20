using ProgrammingLearningApp.Exceptions;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class CustomExceptionTests
	{
		[Fact]
		public void OutOfBoundsException_StoresPosition()
		{
			// Arrange
			var position = new Position(10, 10);

			// Act
			var exception = new OutOfBoundsException(position);

			// Assert
			Assert.Equal(position, exception.AttemptedPosition);
			Assert.Contains("(10,10)", exception.Message);
		}

		[Fact]
		public void BlockedCellException_StoresPosition()
		{
			// Arrange
			var position = new Position(5, 5);

			// Act
			var exception = new BlockedCellException(position);

			// Assert
			Assert.Equal(position, exception.BlockedPosition);
			Assert.Contains("(5,5)", exception.Message);
		}
	}
}