namespace Bari.Core.Model.Discovery
{
    /// <summary>
    /// Interface for discovering suite model data based on convention
    /// </summary>
    public interface ISuiteDiscovery
    {
        /// <summary>
        /// Extends suite model with discovered information based on bari conventions
        /// </summary>
        void ExtendWithDiscoveries();
    }
}