namespace ProgrammingLearningApp.Services
{ 
	public interface IProgramExporter
	{
		void Export(Program program, string filePath);
		string GetFileExtension();
		string GetFormatName();
	}
}