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
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        string Uid { get; }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        void AddToContext(IBuildContext context);

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        ISet<TargetRelativePath> Run(IBuildContext context);

        /// <summary>
        /// Verifies if the builder is able to run. Can be used to fallback to cached results without getting en error.
        /// </summary>
        /// <returns>If <c>true</c>, the builder thinks it can run.</returns>
        bool CanRun();
    }

    /// <summary>
    /// Contracts for <see cref="IBuilder"/> interface
    /// </summary>
    [ContractClassFor(typeof(IBuilder))]
    abstract class IBuilderContracts : IBuilder
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

        public string Uid
        {
            get
            {
                Contract.Ensures(Contract.Result<string>() != null);
                return null; // dummy value 
            }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            Contract.Requires(context != null);
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context"> </param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            Contract.Requires(context != null);
            Contract.Ensures(Contract.Result<ISet<TargetRelativePath>>() != null);
            return null; // dummy value
        }

        /// <summary>
        /// Verifies if the builder is able to run. Can be used to fallback to cached results without getting en error.
        /// </summary>
        /// <returns>If <c>true</c>, the builder thinks it can run.</returns>
        public abstract bool CanRun();
    }
}