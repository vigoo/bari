namespace Bari.Core.Commands.Helper
{
    public interface ICommandTargetParser
    {
        CommandTarget ParseTarget(string target);
    }
}