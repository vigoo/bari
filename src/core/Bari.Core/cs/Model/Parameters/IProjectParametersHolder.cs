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
        /// Gets a possibly inherited parameter block by its name. If the block does not exists, a default
        /// block is returned.
        /// </summary>
        /// <typeparam name="TParams">Parameter block type</typeparam>
        /// <typeparam name="TParamsDef">Inheritable parameter block definition ype</typeparam>
        /// <param name="name">Name of the parameter block</param>
        /// <returns>Returns the composed parameter block</returns>
        TParams GetInheritableParameters<TParams, TParamsDef>(string name)
            where TParamsDef : ProjectParametersPropertyDefs<TParams>, new()
            where TParams : InheritableProjectParameters<TParams, TParamsDef>;

        IInheritableProjectParameters GetInheritableParameters(string name, IInheritableProjectParametersDef def);

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

        public IInheritableProjectParameters GetInheritableParameters(string name, IInheritableProjectParametersDef def)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(def != null);
            Contract.Ensures(Contract.Result<IProjectParameters>() != null);
            return null; // dummy value
        }

        public TParams GetInheritableParameters<TParams, TParamsDef>(string name) 
            where TParamsDef : ProjectParametersPropertyDefs<TParams>, new()
            where TParams : InheritableProjectParameters<TParams, TParamsDef>
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Ensures(Contract.Result<TParams>() != null);
            return default(TParams); // dummy value
        }

        public void AddParameters(string name, IProjectParameters projectParameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Requires(projectParameters != null);
            Contract.Ensures(HasParameters(name));
        }
    }
}