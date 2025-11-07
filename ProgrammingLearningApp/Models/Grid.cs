namespace ProgrammingLearningApp.Models
{
	public class Grid
	{
		private readonly Cell[,] cells;
		public int Width { get; }
		public int Height { get; }
		public Position EndPosition { get; private set; }

		public Grid(int width, int height)
		{
			if (width <= 0 || height <= 0)
				throw new ArgumentException("Grid dimensions must be positive");

			Width = width;
			Height = height;
			cells = new Cell[width, height];

			// Initialize all cells as open
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					cells[x, y] = new Cell();
				}
			}
		}

		public Cell GetCell(int x, int y)
		{
			if (!IsWithinBounds(x, y))
				throw new ArgumentOutOfRangeException($"Position ({x},{y}) is outside grid bounds");

			return cells[x, y];
		}

		public Cell GetCell(Position position)
		{
			return GetCell(position.X, position.Y);
		}

		public void SetCellBlocked(int x, int y, bool blocked)
		{
			if (!IsWithinBounds(x, y))
				throw new ArgumentOutOfRangeException($"Position ({x},{y}) is outside grid bounds");

			cells[x, y].IsBlocked = blocked;
		}

		public void SetEndPosition(int x, int y)
		{
			if (!IsWithinBounds(x, y))
				throw new ArgumentOutOfRangeException($"Position ({x},{y}) is outside grid bounds");

			EndPosition = new Position(x, y);
			cells[x, y].IsEndPosition = true;
		}

		public bool IsWithinBounds(int x, int y)
		{
			return x >= 0 && x < Width && y >= 0 && y < Height;
		}

		public bool IsWithinBounds(Position position)
		{
			return IsWithinBounds(position.X, position.Y);
		}

		public bool IsCellBlocked(int x, int y)
		{
			if (!IsWithinBounds(x, y))
				return true; // Consider out of bounds as blocked

			return cells[x, y].IsBlocked;
		}

		public bool IsCellBlocked(Position position)
		{
			return IsCellBlocked(position.X, position.Y);
		}

		public void Reset()
		{
			for (int x = 0; x < Width; x++)
			{
				for (int y = 0; y < Height; y++)
				{
					cells[x, y].Reset();
				}
			}
		}

		public Position GetNextPosition(Position current, Direction direction)
		{
			int newX = current.X;
			int newY = current.Y;

			switch (direction)
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
	}
}