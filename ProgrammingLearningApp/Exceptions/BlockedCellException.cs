using System;

namespace ProgrammingLearningApp.Exceptions
{
	public class BlockedCellException : Exception
	{
		public Position BlockedPosition { get; }

		public BlockedCellException(Position position)
			: base($"Attempted to move to blocked cell at position {position}")
		{
			BlockedPosition = position;
		}

		public BlockedCellException(Position position, string message)
			: base(message)
		{
			BlockedPosition = position;
		}
	}
}