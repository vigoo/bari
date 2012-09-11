using System.Diagnostics.Contracts;

namespace Bari.Core.Model
{
    /// <summary>
    /// Represents a project of a module, which is a separate processable set of inputs creating one or more targets
    /// 
    /// <example>An example is a set of C# sources compiled into one assembly.</example>
    /// </summary>
    public class Project
    {
        private readonly string name;

        /// <summary>
        /// Gets the project's name
        /// </summary>
        public string Name
        {
            get
            {
                Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));
                
                return name;
            }
        }

        /// <summary>
        /// Constructs the project model instance
        /// </summary>
        /// <param name="name">Name of the project</param>
        public Project(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));

            this.name = name;
        }
    }
}