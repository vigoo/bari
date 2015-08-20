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
        /// Get the builders to be executed before this builder
        /// </summary>
        public virtual IEnumerable<IBuilder> Prerequisites
        {
            get { return new IBuilder[0]; }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public abstract string Uid { get; }

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

        public virtual void AddPrerequisite(IBuilder target)
        {
        }

        public virtual void RemovePrerequisite(IBuilder target)
        {
        }
    }
}