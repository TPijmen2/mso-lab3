using ProgrammingLearningApp.Models;

namespace ProgrammingLearningApp
{
	public interface ICommand
	{
		void Execute(Character character);
		int GetCommandCount();
		int GetMaxNestingLevel();
		int GetRepeatCount();
		string ToString(int indentLevel = 0);
	}
}