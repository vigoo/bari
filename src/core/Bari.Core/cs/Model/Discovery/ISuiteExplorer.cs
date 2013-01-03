namespace Bari.Core.Model.Discovery
{
    /// <summary>
    /// Interface for discovering suite model data based on convention
    /// </summary>
    public interface ISuiteExplorer
    {
        /// <summary>
        /// Extends suite model with discovered information based on bari conventions
        /// </summary>
        /// <param name="suite">The suite model to be extended with discoveries</param>
        void ExtendWithDiscoveries(Suite suite);
    }
}