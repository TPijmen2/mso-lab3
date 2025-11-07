using ProgrammingLearningApp.Exceptions;

namespace ProgrammingLearningApp.Models
{
	public class Character
	{
		public Position Position { get; set; }
		public Direction Direction { get; private set; }
		public List<Position> Path { get; set; }
		public Grid Grid { get; set; }

		public Character(Grid grid = null)
		{
			Grid = grid;
			Reset();
		}

		public void Reset()
		{
			Position = new Position(0, 0);
			Direction = Direction.East;
			Path = new List<Position> { Position.Copy() };
		}

		public void Move(int steps)
		{
			for (int i = 0; i < steps; i++)
			{
				MoveSingleStep();
			}
		}

		private void MoveSingleStep()
		{
			// Calculate next position
			Position nextPosition = CalculateNextPosition();

			// If grid is loaded, perform validation
			if (Grid != null)
			{
				// Check if next position is within bounds
				if (!Grid.IsWithinBounds(nextPosition))
				{
					throw new OutOfBoundsException(nextPosition);
				}

				// Check if next position is blocked
				if (Grid.IsCellBlocked(nextPosition))
				{
					throw new BlockedCellException(nextPosition);
				}

				// Mark cell as visited
				Grid.GetCell(nextPosition).IsVisited = true;
			}

			// Move to next position
			Position = nextPosition;
			Path.Add(Position.Copy());
		}

		private Position CalculateNextPosition()
		{
			int newX = Position.X;
			int newY = Position.Y;

			switch (Direction)
			{
				case Direction.North:
					newY++;
					break;
				case Direction.East:
					newX++;
					break;
				case Direction.South:
					newY--;
					break;
				case Direction.West:
					newX--;
					break;
			}

			return new Position(newX, newY);
		}

		public void TurnLeft()
		{
			Direction = Direction.TurnLeft();
		}

		public void TurnRight()
		{
			Direction = Direction.TurnRight();
		}

		public string GetDirectionString()
		{
			return Direction.ToString().ToLower();
		}

		public void SetGrid(Grid grid)
		{
			Grid = grid;
		}
	}
}