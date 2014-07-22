namespace Bari.Core.Commands.Clean
{
    public interface ICleanParameters
    {
        /// <summary>
        /// If true, references (NuGet, FSRepo, etc.) will no be deleted from the cache
        /// </summary>
        bool KeepReferences { get; } 
    }
}