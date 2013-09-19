namespace Bari.Plugins.FSRepository.Model
{
    public interface IPatternResolutionContext
    {
        /// <summary>
        /// Gets an environment variable
        /// </summary>
        /// <param name="name">Name of the environment variable</param>
        /// <returns>Returns the value of the environment variable or <c>null</c> if it is no available</returns>
        string GetEnvironmentVariable(string name);

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