using ProgrammingLearningApp.Models;
using Xunit;

namespace ProgrammingLearningApp.Tests
{
	public class CharacterTests
	{
		[Fact]
		public void Character_StartsAtOriginFacingEast()
		{
			// Arrange & Act
			var character = new Character();

			// Assert
			Assert.Equal(0, character.Position.X);
			Assert.Equal(0, character.Position.Y);
			Assert.Equal(Direction.East, character.Direction);
		}

		[Fact]
		public void Move_OneStepEast_MovesToCorrectPosition()
		{
			// Arrange
			var character = new Character();

			// Act
			character.Move(1);

			// Assert
			Assert.Equal(1, character.Position.X);
			Assert.Equal(0, character.Position.Y);
		}

		[Fact]
		public void Move_MultipleSteps_MovesCorrectly()
		{
			// Arrange
			var character = new Character();

			// Act
			character.Move(5);

			// Assert
			Assert.Equal(5, character.Position.X);
			Assert.Equal(0, character.Position.Y);
		}

		[Fact]
		public void TurnRight_FromEast_FacesSouth()
		{
			// Arrange
			var character = new Character();

			// Act
			character.TurnRight();

			// Assert
			Assert.Equal(Direction.South, character.Direction);
		}

		[Fact]
		public void TurnLeft_FromEast_FacesNorth()
		{
			// Arrange
			var character = new Character();

			// Act
			character.TurnLeft();

			// Assert
			Assert.Equal(Direction.North, character.Direction);
		}

		[Fact]
		public void TurnRight_FourTimes_ReturnsToOriginalDirection()
		{
			// Arrange
			var character = new Character();

			// Act
			character.TurnRight();
			character.TurnRight();
			character.TurnRight();
			character.TurnRight();

			// Assert
			Assert.Equal(Direction.East, character.Direction);
		}

		[Theory]
		[InlineData(Direction.North, 0, 1)]
		[InlineData(Direction.East, 1, 0)]
		[InlineData(Direction.South, 0, -1)]
		[InlineData(Direction.West, -1, 0)]
		public void Move_InDifferentDirections_MovesCorrectly(Direction direction, int expectedX, int expectedY)
		{
			// Arrange
			var character = new Character();

			// Manually set direction for testing
			while (character.Direction != direction)
			{
				character.TurnRight();
			}

			// Act
			character.Move(1);

			// Assert
			Assert.Equal(expectedX, character.Position.X);
			Assert.Equal(expectedY, character.Position.Y);
		}

		[Fact]
		public void Reset_AfterMovement_ReturnsToOrigin()
		{
			// Arrange
			var character = new Character();
			character.Move(5);
			character.TurnRight();

			// Act
			character.Reset();

			// Assert
			Assert.Equal(0, character.Position.X);
			Assert.Equal(0, character.Position.Y);
			Assert.Equal(Direction.East, character.Direction);
		}

		[Fact]
		public void Path_RecordsMovements()
		{
			// Arrange
			var character = new Character();

			// Act
			character.Move(2);

			// Assert
			Assert.Equal(3, character.Path.Count); // Start position + 2 moves
			Assert.Equal(new Position(0, 0), character.Path[0]);
			Assert.Equal(new Position(1, 0), character.Path[1]);
			Assert.Equal(new Position(2, 0), character.Path[2]);
		}
	}
}