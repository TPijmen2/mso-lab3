namespace ProgrammingLearningApp
{
	public enum Direction
	{
		North,
		East,
		South,
		West
	}

	public static class DirectionExtensions
	{
		public static Direction TurnLeft(this Direction direction)
		{
			return direction switch
			{
				Direction.North => Direction.West,
				Direction.East => Direction.North,
				Direction.South => Direction.East,
				Direction.West => Direction.South,
				_ => throw new ArgumentException("Invalid direction")
			};
		}

		public static Direction TurnRight(this Direction direction)
		{
			return direction switch
			{
				Direction.North => Direction.East,
				Direction.East => Direction.South,
				Direction.South => Direction.West,
				Direction.West => Direction.North,
				_ => throw new ArgumentException("Invalid direction")
			};
		}
	}
}