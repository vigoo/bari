using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Bari.Plugins.FSRepository.Model
{
    /// <summary>
    /// An ordered collection of available <see cref="RepositoryPattern"/> patterns, with the
    /// ability to find the best match for a given context.
    /// </summary>
    public class RepositoryPatternCollection
    {
        private readonly IList<RepositoryPattern> patterns = new List<RepositoryPattern>();
        private readonly IFileSystemRepositoryAccess fsAccess;

        public RepositoryPatternCollection(IFileSystemRepositoryAccess fsAccess)
        {
            Contract.Requires(fsAccess != null);

            this.fsAccess = fsAccess;
        }

        public void AddPattern(RepositoryPattern pattern)
        {
            Contract.Requires(pattern != null);

            patterns.Add(pattern);
        }

        /// <summary>
        /// Resolves the given context using all the available patterns, checking their
        /// resolution in the file system as well.
        /// </summary>
        /// <param name="context">Resolution context</param>
        /// <returns>Returns the path to the dependency if resolution succeeded. Otherwise it returns <c>null</c>.</returns>
        public string Resolve(IPatternResolutionContext context)
        {
            Contract.Requires(context != null);

            return patterns.Select(pattern => pattern.Resolve(context))
                           .FirstOrDefault(res => res != null && fsAccess.Exists(res));
        }
    }
}