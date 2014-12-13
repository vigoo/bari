using System;
using System.Collections.Generic;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;

namespace Bari.Core.Build
{
    /// <summary>
    /// Implementation base for <see cref="IBuilder"/> implementations
    /// </summary>
    /// <typeparam name="TBuilder">Self type</typeparam>
    public abstract class BuilderBase<TBuilder>: IBuilder
        where TBuilder: IBuilder
    {
        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public virtual IDependencies Dependencies { get { return new NoDependencies(); }}

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public abstract string Uid { get; }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public virtual void AddToContext(IBuildContext context)
        {
            context.AddBuilder(this, new IBuilder[0]);
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public abstract ISet<TargetRelativePath> Run(IBuildContext context);

        /// <summary>
        /// Verifies if the builder is able to run. Can be used to fallback to cached results without getting en error.
        /// </summary>
        /// <returns>If <c>true</c>, the builder thinks it can run.</returns>
        public virtual bool CanRun()
        {
            return true;
        }

        /// <summary>
        /// Gets the type of the builder, without any decorators.
        /// </summary>
        /// <value>The type of the builder.</value>
        public virtual Type BuilderType { get { return typeof (TBuilder); }}
    }
}