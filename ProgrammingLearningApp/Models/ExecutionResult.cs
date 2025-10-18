using System.Collections.Generic;

namespace ProgrammingLearningApp
{
	public class ExecutionResult
	{
		public List<string> Trace { get; set; }
		public Position FinalPosition { get; set; }
		public Direction FinalDirection { get; set; }

		public ExecutionResult()
		{
			Trace = new List<string>();
		}

		public override string ToString()
		{
			return $"{string.Join(", ", Trace)}.\nEnd state {FinalPosition} facing {FinalDirection.ToString().ToLower()}.";
		}
	}
}