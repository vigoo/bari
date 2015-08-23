using System.Diagnostics.Contracts;

namespace Bari.Core.Model.Parameters
{
    /// <summary>
    /// Interface for a model item with extensible project parameters (<see cref="IProjectParameters"/>)
    /// </summary>
    [ContractClass(typeof(IProjectParametersHolderContracts))]
    public interface IProjectParametersHolder
    {
        /// <summary>
        /// Checks whether a parameter block exist with the given name
        /// </summary>
        /// <param name="name">Name of the parameter block</param>
        /// <returns>Returns <c>true</c> if a parameter block with the given name is applied to this model item</returns>
        [Pure] bool HasParameters(string name);

        /// <summary>
        /// Gets a parameter block by its name
        /// </summary>
        /// <typeparam name="TParams">The expected type of the parameter block</typeparam>
        /// <param name="name">Name of the parameter block</param>
        /// <returns>Returns the parameter block</returns>
        TParams GetParameters<TParams>(string name)
            where TParams : IProjectParameters;

        /// <summary>
        /// Adds a new parameter block to this model item
        /// </summary>
        /// <param name="name">Name of the parameter block</param>
        /// <param name="projectParameters">The parameter block to be added</param>
        void AddParameters(string name, IProjectParameters projectParameters);
    }

    [ContractClassFor(typeof(IProjectParametersHolder))]
    abstract class IProjectParametersHolderContracts: IProjectParametersHolder
    {        
        public bool HasParameters(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            return false; // dummy value
        }

        public TParams GetParameters<TParams>(string name) where TParams : IProjectParameters
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(HasParameters(name));
            Contract.Ensures(Contract.Result<TParams>() != null);
            return default(TParams); // dummy value
        }

        public void AddParameters(string name, IProjectParameters projectParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(projectParameters != null);
            Contract.Requires(!HasParameters(name));
            Contract.Ensures(HasParameters(name));
        }
    }
}