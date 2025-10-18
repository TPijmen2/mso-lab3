namespace ProgrammingLearningApp.Models
{
	public class Cell
	{
		public bool IsBlocked { get; set; }
		public bool IsEndPosition { get; set; }
		public bool IsVisited { get; set; }

		public Cell(bool isBlocked = false, bool isEndPosition = false)
		{
			IsBlocked = isBlocked;
			IsEndPosition = isEndPosition;
			IsVisited = false;
		}

		public void Reset()
		{
			IsVisited = false;
		}
	}
}