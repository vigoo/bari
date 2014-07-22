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
        /// <param name="parameters"></param>
        void Clean(ICleanParameters parameters);
    }
}