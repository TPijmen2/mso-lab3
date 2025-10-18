namespace ProgrammingLearningApp
{
	public class Position
	{
		public int X { get; set; }
		public int Y { get; set; }

		public Position(int x, int y)
		{
			X = x;
			Y = y;
		}

		public Position Copy()
		{
			return new Position(X, Y);
		}

		public override string ToString()
		{
			return $"({X},{Y})";
		}

		public override bool Equals(object obj)
		{
			if (obj is Position other)
			{
				return X == other.X && Y == other.Y;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(X, Y);
		}
	}
}