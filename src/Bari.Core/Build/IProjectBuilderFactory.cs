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
        /// Adds the builders (<see cref="IBuilder"/>) to the given build context which process
        /// the given set of projects (<see cref="Project"/>)
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="projects">Projects to be built</param>
        IBuilder AddToContext(IBuildContext context, IEnumerable<Project> projects);
    }

    /// <summary>
    /// Contract class for <see cref="IProjectBuilderFactory"/> interface
    /// </summary>
    [ContractClassFor(typeof(IProjectBuilderFactory))]
    abstract class IProjectBuilderFactoryContracts: IProjectBuilderFactory
    {
        /// <summary>
        /// Adds the builders (<see cref="IBuilder"/>) to the given build context which process
        /// the given set of projects (<see cref="Project"/>)
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="projects">Projects to be built</param>
        public IBuilder AddToContext(IBuildContext context, IEnumerable<Project> projects)
        {
            Contract.Requires(context != null);
            Contract.Requires(projects != null);

            return null; // dummy value
        }
    }
}