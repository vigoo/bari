namespace Bari.Core.Commands.Clean
{
    /// <summary>
    /// Interface for extending the clean command
    /// </summary>
    public interface ICleanExtension
    {
        /// <summary>
        /// Performs the additional cleaning step
        /// </summary>
        void Clean();
    }
}