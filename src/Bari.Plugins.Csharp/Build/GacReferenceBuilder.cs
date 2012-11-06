using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// A <see cref="IReferenceBuilder"/> implementation referencing assemblies from the Global Assembly Cache (GAC)
    /// 
    /// <para>
    /// The reference URIs are interpreted in the following way:
    /// 
    /// <example>gac://System.Xml</example>
    /// means that the System.Xml assembly will be directly referenced from the GAC
    /// </para>
    /// </summary>
    public class GacReferenceBuilder : IReferenceBuilder
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (GacReferenceBuilder));

        private Reference reference;

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return new NoDependencies(); }
        }

        public string Uid
        {
            get { return reference.Uri.Host; }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {            
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context"> </param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            log.DebugFormat("Resolving reference {0}", reference.Uri);

            // The "GAC!" prefix is handled by the .NET related builders correctly.
            // This reference builder would not be useful for other builders anyway:
            return new HashSet<TargetRelativePath>(new[]
                {
                    new TargetRelativePath("GAC!" + reference.Uri.Host)
                });
        }

        /// <summary>
        /// Gets or sets the reference to be resolved
        /// </summary>
        public Reference Reference
        {
            get { return reference; }
            set { reference = value; }
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("[{0}]", reference.Uri);
        }
    }
}