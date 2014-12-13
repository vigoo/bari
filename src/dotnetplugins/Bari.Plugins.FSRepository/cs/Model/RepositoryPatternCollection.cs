using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Bari.Core.Model;

namespace Bari.Plugins.FSRepository.Model
{
    /// <summary>
    /// An ordered collection of available <see cref="RepositoryPattern"/> patterns, with the
    /// ability to find the best match for a given context.
    /// </summary>
    public class RepositoryPatternCollection: IProjectParameters
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (RepositoryPatternCollection));

        private readonly IList<RepositoryPattern> patterns = new List<RepositoryPattern>();
        private readonly IFileSystemRepositoryAccess fsAccess;

        public static readonly RepositoryPatternCollection Empty = new RepositoryPatternCollection();

        /// <summary>
        /// Gets the list of registered patterns, in resolve order
        /// </summary>
        public IEnumerable<RepositoryPattern> Patterns
        {
            get { return patterns; }
        }

        private RepositoryPatternCollection()
        {
            // Only used for the Empty collection
        }

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
        public PatternResolution Resolve(IPatternResolutionContext context)
        {
            Contract.Requires(context != null);

            var logLines = new StringBuilder();

            foreach (RepositoryPattern pattern in patterns)
            {
                string res = pattern.Resolve(context);

                if (res != null)
                {
                    logLines.AppendFormat("Trying resolved FS repository path: {0}\n", res);

                    if (fsAccess.Exists(res))
                        return PatternResolution.Success(res);
                    else
                        logLines.AppendFormat("FS repository path `{0}` is invalid\n", res);
                }
            }

            return PatternResolution.Failure(logLines.ToString());
        }
    }
}