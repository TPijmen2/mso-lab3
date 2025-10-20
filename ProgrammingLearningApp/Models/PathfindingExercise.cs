using System;

namespace ProgrammingLearningApp.Models
{
	public class PathfindingExercise
	{
		public string Name { get; }
		public Grid Grid { get; }
		public Position StartPosition { get; }

		public PathfindingExercise(string name, Grid grid, Position startPosition = null)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Exercise name cannot be empty");

			Name = name;
			Grid = grid ?? throw new ArgumentNullException(nameof(grid));
			StartPosition = startPosition ?? new Position(0, Grid?.Height - 1 ?? 0);

			// Validate start position is within bounds
			if (!grid.IsWithinBounds(StartPosition))
				throw new ArgumentException("Start position must be within grid bounds");
		}

		public bool IsCompleted(Position currentPosition)
		{
			return currentPosition.Equals(Grid.EndPosition);
		}

		public void Reset()
		{
			Grid.Reset();
		}
	}
}