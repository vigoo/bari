namespace Bari.Core.Commands.Clean
{
    public interface ICleanParameters
    {
        /// <summary>
        /// If true, references (NuGet, FSRepo, etc.) will no be deleted from the cache
        /// </summary>
        bool KeepReferences { get; }

        /// <summary>
        /// Checks whether the given parameter turns on the <see cref="KeepReferences" option/>
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>True if it would turn on the <see cref="KeepReferences"/> option</returns>
        bool IsKeepReferencesParameter(string parameter);
    }
}