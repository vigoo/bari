namespace Bari.Core.Commands.Clean
{
    public interface ICleanParameters
    {
        /// <summary>
        /// If true, references (NuGet, FSRepo, etc.) will no be deleted from the cache
        /// </summary>
        bool KeepReferences { get; }

        /// <summary>
        /// If true, clean won't delete some files not directly related to the target of the build
        /// </summary>
        bool SoftClean { get; }

        /// <summary>
        /// Checks whether the given parameter turns on the <see cref="KeepReferences"/> option
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>True if it would turn on the <see cref="KeepReferences"/> option</returns>
        bool IsKeepReferencesParameter(string parameter);

        /// <summary>
        /// Checks whether the given parameter turns on the <see cref="SoftClean"/> option
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>True if it would turn on the <see cref="SoftClean"/> option</returns>
        bool IsSoftCleanParameter(string parameter);
    }
}