namespace Bari.Core.Commands.Helper
{
    public interface IHasBuildTarget
    {
        /// <summary>
        /// Gets the build target passed to the command
        /// </summary>
        string BuildTarget { get; } 
    }
}