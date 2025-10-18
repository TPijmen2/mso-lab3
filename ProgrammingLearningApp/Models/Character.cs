using System.Collections.Generic;

namespace ProgrammingLearningApp
{
	public class Character
	{
		public Position Position { get; private set; }
		public Direction Direction { get; private set; }
		public List<Position> Path { get; private set; }

		public Character()
		{
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
				switch (Direction)
				{
					case Direction.North:
						Position.Y++;
						break;
					case Direction.East:
						Position.X++;
						break;
					case Direction.South:
						Position.Y--;
						break;
					case Direction.West:
						Position.X--;
						break;
				}
				Path.Add(Position.Copy());
			}
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
	}
}