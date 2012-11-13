using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.Build
{
    public class InSolutionReferenceBuilder: IReferenceBuilder
    {
        private readonly Project project;
        private Reference reference;

        public InSolutionReferenceBuilder(Project project)
        {
            this.project = project;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return new NoDependencies(); }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get { return project.Module.Name + "." + project.Name; }
        }

        /// <summary>
        /// Prepares a builder to be ran in a given build context.
        /// 
        /// <para>This is the place where a builder can add additional dependencies.</para>
        /// </summary>
        /// <param name="context">The current build context</param>
        public void AddToContext(IBuildContext context)
        {
            context.AddBuilder(this, new IBuilder[0]);
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {
            // TODO
            return new HashSet<TargetRelativePath>(
                new[]
                    {
                        new TargetRelativePath("SLN!"+Uid), 
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
            return string.Format("[sln-ref:{0}]", Uid);
        }
    }
}