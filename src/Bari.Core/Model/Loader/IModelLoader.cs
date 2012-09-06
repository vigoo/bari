using System.Diagnostics.Contracts;

namespace Bari.Core.Model.Loader
{
    /// <summary>
    /// Model loader interface
    /// 
    /// <para>Model loaders are responsible for reading a full application suite's definition.</para>
    /// <para>Multiple model loader implementations can exist simultaneously, and each implementation can
    /// decide for each request whether it supports it or not.</para>
    /// </summary>
    [ContractClass(typeof(IModelLoaderContracts))]
    public interface IModelLoader
    {
        /// <summary>
        /// Returns true if the loader can load suite model from the given source
        /// </summary>
        /// <param name="source">Source, can mean anything (file names, urls, markup, etc.)</param>
        /// <returns>Returns <c>true</c> if the source has been identified and can be loaded.</returns>
        [Pure]
        bool Supports(string source);

        /// <summary>
        /// Loads a suite model from the given source
        /// 
        /// <para>It is guaranteed that this method will only be called if <see cref="Supports"/> return <c>true</c>
        /// for the same source.</para>
        /// </summary>
        /// <param name="source">Source, can mean anything (file names, urls, markup, etc.)</param>
        /// <returns>Returns the loaded suite model. Never returns <c>null</c>. On error it throws an exception.</returns>
        Suite Load(string source);
    }

    /// <summary>
    /// Contracts for the <see cref="IModelLoader"/> interface
    /// </summary>
    [ContractClassFor(typeof(IModelLoader))]
    public abstract class IModelLoaderContracts: IModelLoader
    {
        /// <summary>
        /// Returns true if the loader can load suite model from the given source
        /// </summary>
        /// <param name="source">Source, can mean anything (file names, urls, markup, etc.)</param>
        /// <returns>Returns <c>true</c> if the source has been identified and can be loaded.</returns>        
        public bool Supports(string source)
        {
            Contract.Requires(source != null);
            return false; // dummy value
        }

        /// <summary>
        /// Loads a suite model from the given source
        /// 
        /// <para>It is guaranteed that this method will only be called if <see cref="IModelLoader.Supports"/> return <c>true</c>
        /// for the same source.</para>
        /// </summary>
        /// <param name="source">Source, can mean anything (file names, urls, markup, etc.)</param>
        /// <returns>Returns the loaded suite model. Never returns <c>null</c>. On error it throws an exception.</returns>
        public Suite Load(string source)
        {
            Contract.Requires(source != null);
            Contract.Requires(Supports(source));
            Contract.Ensures(Contract.Result<Suite>() != null);

            return null; // dummy value
        }
    }
}