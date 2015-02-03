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

        /// <summary>
        /// Defines a run-time environment variable
        /// 
        /// It overrides any system variable with the same name.
        /// </summary>
        /// <param name="name">Name of the environment variable</param>
        /// <param name="value">It's value</param>
        void Define(string name, string value);

        /// <summary>
        /// Undefines a run-time environment variable
        /// </summary>
        /// <param name="name">Name of the environment variable</param>
        void Undefine(string name);
    }
}