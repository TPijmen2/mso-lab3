namespace ProgrammingLearningApp.Exceptions
{
	public class OutOfBoundsException : Exception
	{
		public Position AttemptedPosition { get; }

		public OutOfBoundsException(Position position)
			: base($"Attempted to move to position {position} which is outside the grid bounds")
		{
			AttemptedPosition = position;
		}

		public OutOfBoundsException(Position position, string message)
			: base(message)
		{
			AttemptedPosition = position;
		}
	}
}