namespace ProgrammingLearningApp.Services
{
	/// <summary>
	/// Strategy pattern interface for exporting programs to different formats
	/// </summary>
	public interface IProgramExporter
	{
		void Export(Program program, string filePath);
		string GetFileExtension();
		string GetFormatName();
	}
}