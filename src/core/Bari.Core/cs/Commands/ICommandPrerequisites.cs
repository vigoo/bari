namespace Bari.Core.Commands
{
    public interface ICommandPrerequisites
    {
        bool RequiresSuite { get; } 
    }
}