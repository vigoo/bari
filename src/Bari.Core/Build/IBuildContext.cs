using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Bari.Core.Generic;
using Bari.Core.Generic.Graph;

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
        /// Adds a new graph transformation which will be executed before the builders
        /// </summary>
        /// <param name="transformation">Transformation function, returns <c>false</c> to cancel the build process</param>
        void AddTransformation(Func<List<IDirectedGraphEdge<IBuilder>>, bool> transformation);

        /// <summary>
        /// Runs all the added builders
        /// </summary>
        /// <returns>Returns the union of result paths given by all the builders added to the context</returns>
        ISet<TargetRelativePath> Run();

        /// <summary>
        /// Gets the result paths returned by the given builder if it has already ran. Otherwise it throws an
        /// exception.
        /// </summary>
        /// <param name="builder">Builder which was added previously with <see cref="AddBuilder"/> and was already executed.</param>
        /// <returns>Return the return value of the builder's <see cref="IBuilder.Run"/> method.</returns>
        ISet<TargetRelativePath> GetResults(IBuilder builder);

        /// <summary>
        /// Dumps the build context to dot files
        /// </summary>
        /// <param name="builderGraphStream">Stream where the builder graph will be dumped</param>
        void Dump(Stream builderGraphStream);
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
        /// Adds a new graph transformation which will be executed before the builders
        /// </summary>
        /// <param name="transformation">Transformation function, returns <c>false</c> to cancel the build process</param>
        public void AddTransformation(Func<List<IDirectedGraphEdge<IBuilder>>, bool> transformation)
        {
            Contract.Requires(transformation != null);
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

        /// <summary>
        /// Gets the result paths returned by the given builder if it has already ran. Otherwise it throws an
        /// exception.
        /// </summary>
        /// <param name="builder">Builder which was added previously with <see cref="IBuildContext.AddBuilder"/> and was already executed.</param>
        /// <returns>Return the return value of the builder's <see cref="IBuilder.Run"/> method.</returns>
        public ISet<TargetRelativePath> GetResults(IBuilder builder)
        {
            Contract.Requires(builder != null);
            Contract.Ensures(Contract.Result<ISet<TargetRelativePath>>() != null);

            return null; // dummy value
        }

        /// <summary>
        /// Dumps the build context to dot files
        /// </summary>
        /// <param name="builderGraphStream">Stream where the builder graph will be dumped</param>
        public void Dump(Stream builderGraphStream)
        {
            Contract.Requires(builderGraphStream != null);
        }
    }
}