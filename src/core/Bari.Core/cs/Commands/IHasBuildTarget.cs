namespace Bari.Core.Commands
{
    public interface IHasBuildTarget
    {
        /// <summary>
        /// Gets the build target passed to the command
        /// </summary>
        string BuildTarget { get; } 
    }
}