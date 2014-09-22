namespace Bari.Core.Generic
{
    public interface IEnvironmentVariableContext
    {
        /// <summary>
        /// Gets an environment variable
        /// </summary>
        /// <param name="name">Name of the environment variable</param>
        /// <returns>Returns the value of the environment variable or <c>null</c> if it is no available</returns>
        string GetEnvironmentVariable(string name); 
    }
}