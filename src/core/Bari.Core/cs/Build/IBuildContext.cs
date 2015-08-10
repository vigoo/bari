using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Bari.Core.Generic;
using QuickGraph;
using Bari.Core.UI;

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
        void AddBuilder(IBuilder builder);

        /// <summary>
        /// Adds a new graph transformation which will be executed before the builders
        /// </summary>
        /// <param name="transformation">Transformation function, returns <c>false</c> to cancel the build process</param>
        void AddTransformation(Func<ISet<EquatableEdge<IBuilder>>, bool> transformation);

        /// <summary>
        /// Runs all the added builders
        /// </summary>
        /// <param name="rootBuilder">The root builder which represents the final goal of the build process.
        /// If specified, every branch which is not accessible from the root builder will be removed
        /// from the build graph before executing it.</param>
        /// <returns>Returns the union of result paths given by all the builders added to the context</returns>
        ISet<TargetRelativePath> Run(IBuilder rootBuilder = null);

        /// <summary>
        /// Gets the result paths returned by the given builder if it has already ran. Otherwise it throws an
        /// exception.
        /// </summary>
        /// <param name="builder">Builder which was added previously with <see cref="AddBuilder"/> and was already executed.</param>
        /// <returns>Return the return value of the builder's <see cref="IBuilder.Run"/> method.</returns>
        ISet<TargetRelativePath> GetResults(IBuilder builder);

        /// <summary>
        /// Gets the dependent builders of a given builder
        /// </summary>
        /// <param name="builder">Builder to get dependencies of</param>
        /// <returns>A possibly empty enumeration of builders</returns>
        IEnumerable<IBuilder> GetDependencies(IBuilder builder);

        /// <summary>
        /// Dumps the build context to dot files
        /// </summary>
        /// <param name="builderGraphStream">Stream where the builder graph will be dumped</param>
        /// <param name="rootBuilder">The root builder</param>
        void Dump(Stream builderGraphStream, IBuilder rootBuilder);

        /// <summary>
        /// Dumps the dependencies of the builder
        /// </summary>
        /// <param name="rootBuilder">The root builder</param>
        /// <param name="output">Output to dump information to</param>
        void DumpDependencies(IBuilder rootBuilder, IUserOutput output);

        /// <summary>
        /// Checks whether the given builder was already added to the context
        /// </summary>
        /// <param name="builder">Builder to look for</param>
        /// <returns>Returns <c>true</c> if the builder is added to the context</returns>
        bool Contains(IBuilder builder);

        /// <summary>
        /// Gets the registered effective builder instance for a given builder at the given context
        /// </summary>
        /// <param name="builder">Builder to resolve</param>
        /// <returns>Returns the builer itself or its transformed form</returns>
        IBuilder GetEffectiveBuilder(IBuilder builder);

        /// <summary>
        /// Gets all the result files under the given subdirectory of target root
        /// </summary>
        /// <param name="targetDir">Subdirectory of target</param>
        /// <returns>An enumeration of target relative paths all pointing to files 
        /// generated under the current build context to the given subdirectory or one of 
        /// its children.</returns>
        IEnumerable<TargetRelativePath> GetAllResultsIn(TargetRelativePath targetDir);

        /// <summary>
        /// Gets the root context instance
        /// </summary>
        IBuildContext RootContext { get; }
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
        public void AddBuilder(IBuilder builder)
        {
            Contract.Requires(builder != null);
        }

        /// <summary>
        /// Adds a new graph transformation which will be executed before the builders
        /// </summary>
        /// <param name="transformation">Transformation function, returns <c>false</c> to cancel the build process</param>
        public void AddTransformation(Func<ISet<EquatableEdge<IBuilder>>, bool> transformation)
        {
            Contract.Requires(transformation != null);
        }

        /// <summary>
        /// Runs all the added builders
        /// </summary>
        /// <param name="rootBuilder"> </param>
        /// <returns>Returns the union of result paths given by all the builders added to the context</returns>
        public ISet<TargetRelativePath> Run(IBuilder rootBuilder)
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
        /// Gets the dependent builders of a given builder
        /// </summary>
        /// <param name="builder">Builder to get dependencies of</param>
        /// <returns>A possibly empty enumeration of builders</returns>
        public IEnumerable<IBuilder> GetDependencies(IBuilder builder)
        {
            Contract.Requires(builder != null);
            Contract.Ensures(Contract.Result<IEnumerable<IBuilder>>() != null);

            return null; // dummy
        }

        /// <summary>
        /// Dumps the build context to dot files
        /// </summary>
        /// <param name="builderGraphStream">Stream where the builder graph will be dumped</param>
        /// <param name="rootBuilder">The root builder</param>
        public void Dump(Stream builderGraphStream, IBuilder rootBuilder)
        {
            Contract.Requires(builderGraphStream != null);
        }

        ///<summary>
        ///Dumps the dependencies of the builder
        ///</summary>
        ///<param name="rootBuilder">The root builder</param>
        ///<param name="output">Output to dump information to</param>
        public void DumpDependencies(IBuilder rootBuilder, IUserOutput output)
        {
            Contract.Requires(rootBuilder != null);
            Contract.Requires(output != null);
        }

        /// <summary>
        /// Checks whether the given builder was already added to the context
        /// </summary>
        /// <param name="builder">Builder to look for</param>
        /// <returns>Returns <c>true</c> if the builder is added to the context</returns>
        public bool Contains(IBuilder builder)
        {
            Contract.Requires(builder != null);
            return false; // dummy
        }

        /// <summary>
        /// Gets the registered effective builder instance for a given builder at the given context
        /// </summary>
        /// <param name="builder">Builder to resolve</param>
        /// <returns>Returns the builer itself or its transformed form</returns>
        public IBuilder GetEffectiveBuilder(IBuilder builder)
        {
            Contract.Ensures(Contract.Result<IBuilder>() != null);
            return null; // dummy
        }

        public IEnumerable<TargetRelativePath> GetAllResultsIn(TargetRelativePath targetDir)
        {
            Contract.Requires(targetDir != null);
            Contract.Ensures(Contract.Result<IEnumerable<TargetRelativePath>>() != null);

            return null; // dummy
        }

        public IBuildContext RootContext
        {
            get 
            { 
                Contract.Ensures(Contract.Result<IBuildContext>() != null);
                return null; // dummy
            }
        }
    }
}