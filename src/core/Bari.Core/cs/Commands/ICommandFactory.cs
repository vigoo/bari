namespace Bari.Core.Commands
{
    public interface ICommandFactory
    {
        ICommand CreateCommand(string name);
    }
}