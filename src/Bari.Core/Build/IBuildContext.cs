using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Bari.Core.Generic;

namespace Bari.Core.Build
{
    /// <summary>
    /// Build context provides an interface to add additional build steps to a build process
    /// </summary>
    [ContractClass(typeof(IBuildContextContracts))]
    public interface IBuildContext
    {
        /// <summary>
        /// Adds a new builder to be executed to the context
        /// </summary>
        /// <param name="builder">The builder to be executed</param>
        /// <param name="prerequisites">Builder's prerequisites. The prerequisites must be added
        /// separately with the <see cref="AddBuilder"/> method, listing them here only changes the
        /// order in which they are executed.</param>
        void AddBuilder(IBuilder builder, IEnumerable<IBuilder> prerequisites);

        /// <summary>
        /// Runs all the added builders
        /// </summary>
        /// <returns>Returns the union of result paths given by all the builders added to the context</returns>
        ISet<TargetRelativePath> Run();
    }

    /// <summary>
    /// Contracts for <see cref="IBuildContext"/>
    /// </summary>
    [ContractClassFor(typeof(IBuildContext))]
    abstract class IBuildContextContracts: IBuildContext
    {
        /// <summary>
        /// Adds a new builder to be executed to the context
        /// </summary>
        /// <param name="builder">The builder to be executed</param>
        /// <param name="prerequisites">Builder's prerequisites. The prerequisites must be added
        /// separately with the <see cref="IBuildContext.AddBuilder"/> method, listing them here only changes the
        /// order in which they are executed.</param>
        public void AddBuilder(IBuilder builder, IEnumerable<IBuilder> prerequisites)
        {
            Contract.Requires(builder != null);
            Contract.Requires(prerequisites != null);
        }

        /// <summary>
        /// Runs all the added builders
        /// </summary>
        /// <returns>Returns the union of result paths given by all the builders added to the context</returns>
        public ISet<TargetRelativePath> Run()
        {
            Contract.Ensures(Contract.Result<ISet<TargetRelativePath>>() != null);

            return null; // dummy value
        }
    }
}