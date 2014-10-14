namespace Bari.Core.Commands
{
    public interface ICommandFactory
    {
        ICommandPrerequisites CreateCommandPrerequisites(string name);
        ICommand CreateCommand(string name);
    }
}