using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Nuget.Tools;

namespace Bari.Plugins.Nuget.Build
{
    public class NugetReferenceBuilder: IReferenceBuilder
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (NugetReferenceBuilder));

        private readonly INuGet nuget;
        private readonly IFileSystemDirectory targetRoot;

        private Reference reference;

        public NugetReferenceBuilder(INuGet nuget, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.nuget = nuget;
            this.targetRoot = targetRoot;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return new NoDependencies(); }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run()
        {
            log.DebugFormat("Resolving reference {0}", reference.Uri);

            targetRoot.CreateDirectory("deps");
            return new HashSet<TargetRelativePath>(
                nuget.InstallPackage(reference.Uri.Host, targetRoot, "deps"));
        }

        public Reference Reference
        {
            get { return reference; }
            set { reference = value; }
        }
    }
}