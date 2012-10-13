using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Generic;

namespace Bari.Core.Build
{
    /// <summary>
    /// Builder represents a set of the build process where a given set of dependencies
    /// is used to create a set of outputs.
    /// </summary>
    [ContractClass(typeof(IBuilderContracts))]
    public interface IBuilder
    {
        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        IDependencies Dependencies { get; }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        ISet<TargetRelativePath> Run();
    }

    /// <summary>
    /// Contracts for <see cref="IBuilder"/> interface
    /// </summary>
    [ContractClassFor(typeof(IBuilder))]
    abstract class IBuilderContracts: IBuilder
    {
        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get
            {
                Contract.Ensures(Contract.Result<IDependencies>() != null);
                return null; // dummy value
            }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run()
        {
            Contract.Ensures(Contract.Result<ISet<TargetRelativePath>>() != null);
            return null; // dummy value
        }
    }
}