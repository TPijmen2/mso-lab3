namespace ProgrammingLearningApp
{
	public class ProgramMetrics
	{
		public int CommandCount { get; set; }
		public int MaxNestingLevel { get; set; }
		public int RepeatCount { get; set; }

		public override string ToString()
		{
			return $"Commands: {CommandCount}, Max Nesting: {MaxNestingLevel}, Repeats: {RepeatCount}";
		}
	}
}