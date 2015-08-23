using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// Interface for factory classes which support building projects
    /// </summary>
    [ContractClass(typeof(IProjectBuilderFactoryContracts))]
    public interface IProjectBuilderFactory
    {
        /// <summary>
        /// Creates a builder (<see cref="IBuilder"/>) which process
        /// the given set of projects (<see cref="Project"/>)
        /// </summary>
        /// <param name="projects">Projects to be built</param>
        IBuilder Create(IEnumerable<Project> projects);
    }

    /// <summary>
    /// Contract class for <see cref="IProjectBuilderFactory"/> interface
    /// </summary>
    [ContractClassFor(typeof(IProjectBuilderFactory))]
    abstract class IProjectBuilderFactoryContracts: IProjectBuilderFactory
    {
        /// <summary>
        /// Creates a builder (<see cref="IBuilder"/>) which process
        /// the given set of projects (<see cref="Project"/>)
        /// </summary>
        /// <param name="projects">Projects to be built</param>
        public IBuilder Create(IEnumerable<Project> projects)
        {
            Contract.Requires(projects != null);

            return null; // dummy value
        }
    }
}