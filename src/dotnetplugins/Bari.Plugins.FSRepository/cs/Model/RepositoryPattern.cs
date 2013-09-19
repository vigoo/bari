using System.Diagnostics.Contracts;
using System.Text;
using System.Text.RegularExpressions;

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
        private readonly Regex envVarsRegex = new Regex(@"\$([a-zA-Z0-9]+)", RegexOptions.Singleline);

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

            if (!ResolveEnvironmentVariables(context, resultBuilder)) 
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

        private bool ResolveEnvironmentVariables(IPatternResolutionContext context, StringBuilder resultBuilder)
        {
            Match match;
            do
            {
                match = envVarsRegex.Match(resultBuilder.ToString());

                if (match.Success)
                {
                    string value = context.GetEnvironmentVariable(match.Groups[1].Captures[0].Value);
                    if (value == null)
                    {
                        log.InfoFormat("Failed to resolve FS repository pattern {0}: environment variable {1} does not exists", pattern, match.Value);
                        return false;
                    }
                    else
                    {
                        resultBuilder.Remove(match.Index, match.Length);
                        resultBuilder.Insert(match.Index, value);
                    }
                }
            } while (match.Success);
            
            return true;
        }
    }
}