using Bari.Core.Generic;

namespace Bari.Plugins.FSRepository.Model
{
    public interface IPatternResolutionContext: IEnvironmentVariableContext
    {        
        /// <summary>
        /// Gets the repository name, if specified, otherwise <c>null</c>
        /// </summary>
        string RepositoryName { get; }

        /// <summary>
        /// Gets the name of the dependency
        /// </summary>
        string DependencyName { get; }

        /// <summary>
        /// Gets the file name without extension
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Gets the file name's extension
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Gets the version, or <c>null</c> if it is not available
        /// </summary>
        string Version { get; }
    }
}