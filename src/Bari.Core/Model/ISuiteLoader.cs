using System.Diagnostics.Contracts;

namespace Bari.Core.Model
{
    /// <summary>
    /// Interface for creating a suite specification model from an arbitrary source
    /// </summary>
    [ContractClass(typeof(ISuiteLoaderContracts))]
    public interface ISuiteLoader
    {
        /// <summary>
        /// Loads a suite from a given source using the first matching model loader implementation
        /// </summary>
        /// <param name="source">The suite specification source</param>
        /// <returns>Returns the loaded suite model. On error it throws an exception, never returns <c>null</c>.</returns>
        Suite Load(string source);
    }

    /// <summary>
    /// Contracts for the <see cref="ISuiteLoader"/> interface
    /// </summary>
    [ContractClassFor(typeof(ISuiteLoader))]
    public abstract class ISuiteLoaderContracts: ISuiteLoader
    {
        /// <summary>
        /// Loads a suite from a given source using the first matching model loader implementation
        /// </summary>
        /// <param name="source">The suite specification source</param>
        /// <returns>Returns the loaded suite model. On error it throws an exception, never returns <c>null</c>.</returns>
        public Suite Load(string source)
        {
            Contract.Requires(source != null);
            Contract.Ensures(Contract.Result<Suite>() != null);

            return null; // dummy value
        }
    }
}