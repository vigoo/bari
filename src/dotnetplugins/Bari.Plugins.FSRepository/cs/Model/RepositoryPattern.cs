using System.Diagnostics.Contracts;
using System.Text;
using Bari.Core.Generic;

namespace Bari.Plugins.FSRepository.Model
{
    /// <summary>
    /// Represents a file-system repository pattern, and implements its resolution
    ///
    /// <para>
    /// The pattern supports the following placeholders:
    /// - <c>$ENVVAR</c> replaced to the value of ENVVAR environment variable, if exists.
    /// - <c>%NAME</c> replaced to the name of the dependency
    /// - <c>%FILENAME</c> replaced to the file name, without extension
    /// - <c>%VERSION</c> replaced to the requested version, if any, otherwise empty string
    /// - <c>%EXT</c> replaced to the file name's extension
    /// </para>
    /// </summary>
    public class RepositoryPattern
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (RepositoryPattern));

        private readonly string pattern;        

        /// <summary>
        /// Gets the repository pattern
        /// </summary>
        public string Pattern
        {
            get { return pattern; }
        }

        /// <summary>
        /// Defines a pattern
        /// </summary>
        /// <param name="pattern">The pattern as read from the suite definition</param>
        public RepositoryPattern(string pattern)
        {
            Contract.Requires(pattern != null);

            this.pattern = pattern;
        }

        /// <summary>
        /// Tries to resolve the pattern using the given context
        /// </summary>
        /// <param name="context">The context to get variables from</param>
        /// <returns>Returns the resolved path to the dependency, or <c>null</c> if
        /// the resolution is not possible.</returns>
        public string Resolve(IPatternResolutionContext context)
        {
            Contract.Requires(context != null);

            var resultBuilder = new StringBuilder(pattern);

            if (!EnvironmentVariables.ResolveEnvironmentVariables(context, resultBuilder, 
                m => log.InfoFormat("Failed to resolve FS repository pattern {0}: environment variable {1} does not exists", pattern, m)))
                return null;

            resultBuilder.Replace("%NAME", context.DependencyName);
            resultBuilder.Replace("%FILENAME", context.FileName);
            resultBuilder.Replace("%EXT", context.Extension);
            if (!string.IsNullOrEmpty(context.Version))
                resultBuilder.Replace("%VERSION", context.Version);
            else
            {                
                if (resultBuilder.ToString().IndexOf("%VERSION", System.StringComparison.InvariantCulture) >= 0)
                    // Pattern requires version but we don't have it
                    return null;
            }

            return resultBuilder.ToString();
        }
    }
}