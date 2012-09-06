using System;
using Bari.Core.Exceptions;
using Bari.Core.Model.Loader;
using Ninject;
using Ninject.Syntax;

namespace Bari.Core.Model
{
    /// <summary>
    /// Default implementation of the <see cref="ISuiteLoader"/> interface
    /// </summary>
    public class DefaultSuiteLoader : ISuiteLoader
    {
        private readonly IResolutionRoot root;

        /// <summary>
        /// Creates the default suite loader
        /// </summary>
        /// <param name="root">Path to resolve instances</param>
        public DefaultSuiteLoader(IResolutionRoot root)
        {
            this.root = root;
        }

        /// <summary>
        /// Loads a suite from a given source using the first matching model loader implementation
        /// </summary>
        /// <param name="source">The suite specification source</param>
        /// <returns>Returns the loaded suite model. On error it throws an exception, never returns <c>null</c>.</returns>
        public Suite Load(string source)
        {
            foreach (var loader in root.GetAll<IModelLoader>())
            {
                if (loader.Supports(source))
                    return loader.Load(source);
            }

            throw new NoPluginForTaskException(
                String.Format("No plugin could interpret suite definition source {0}", source));
        }
    }
}