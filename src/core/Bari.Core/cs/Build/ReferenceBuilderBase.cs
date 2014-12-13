using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// Base class for implementing <see cref="IReferenceBuilder"/> builders
    /// </summary>
    /// <typeparam name="TReferenceBuilder">Self type</typeparam>
    public abstract class ReferenceBuilderBase<TReferenceBuilder>: BuilderBase<TReferenceBuilder>, IReferenceBuilder
        where TReferenceBuilder: IReferenceBuilder
    {
        /// <summary>
        /// Gets or sets the reference to be resolved
        /// </summary>
        public abstract Reference Reference { get; set; }

        /// <summary>
        /// If <c>false</c>, the reference builder can be ignored as an optimization
        /// </summary>
        public virtual bool IsEffective
        {
            get { return true; }
        }
    }
}