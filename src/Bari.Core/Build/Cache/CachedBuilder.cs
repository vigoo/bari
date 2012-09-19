using System;
using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Core.Build.Cache
{
    /// <summary>
    /// Wraps a builder so it is only ran if its dependencies has been modified,
    /// or the cache has been corrupted.
    /// </summary>
    public class CachedBuilder: IBuilder
    {
        private readonly IBuilder wrappedBuilder;

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return wrappedBuilder.Dependencies; }
        }

        /// <summary>
        /// Creates a cached builder
        /// </summary>
        /// <param name="wrappedBuilder">The builder instance to be wrapped</param>
        public CachedBuilder(IBuilder wrappedBuilder)
        {
            this.wrappedBuilder = wrappedBuilder;
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <returns>Returns a set of generated files, in suite relative paths</returns>
        public ISet<TargetRelativePath> Run()
        {
            // TODO: get cached dependencies from cache, compare fingerprint with current one and update cache if needed using the wrapped builder
            throw new NotImplementedException();
        }
    }
}