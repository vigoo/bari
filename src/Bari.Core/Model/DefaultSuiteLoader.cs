using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Exceptions;
using Bari.Core.Model.Loader;

namespace Bari.Core.Model
{
    /// <summary>
    /// Default implementation of the <see cref="ISuiteLoader"/> interface
    /// </summary>
    public class DefaultSuiteLoader : ISuiteLoader
    {
        private readonly IEnumerable<IModelLoader> loaders;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(loaders != null);
        }

        /// <summary>
        /// Creates the default suite loader
        /// </summary>
        /// <param name="loaders">All the registerd module loader implementations</param>
        public DefaultSuiteLoader(IEnumerable<IModelLoader> loaders)
        {
            Contract.Requires(loaders != null);
            Contract.Ensures(this.loaders == loaders);

            this.loaders = loaders;
        }

        /// <summary>
        /// Loads a suite from a given source using the first matching model loader implementation
        /// </summary>
        /// <param name="source">The suite specification source</param>
        /// <returns>Returns the loaded suite model. On error it throws an exception, never returns <c>null</c>.</returns>
        public Suite Load(string source)
        {
            foreach (var loader in loaders)
            {
                if (loader.Supports(source))
                    return loader.Load(source);
            }

            throw new NoPluginForTaskException(
                String.Format("No plugin could interpret suite definition source {0}", source));
        }
    }
}