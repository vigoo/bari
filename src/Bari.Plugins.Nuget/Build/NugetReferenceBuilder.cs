using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Nuget.Tools;

namespace Bari.Plugins.Nuget.Build
{
    /// <summary>
    /// A <see cref="IReferenceBuilder"/> implementation downloading references with the NuGet package manager
    /// 
    /// <para>
    /// The reference URIs are interpreted in the following way:
    /// 
    /// <example>nuget://Ninject</example>
    /// means that the latest version of the Ninject package should be downloaded and added as a reference.
    /// </para>
    /// </summary>
    public class NugetReferenceBuilder: IReferenceBuilder
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (NugetReferenceBuilder));

        private readonly INuGet nuget;
        private readonly IFileSystemDirectory targetRoot;

        private Reference reference;

        /// <summary>
        /// Constructs the builder
        /// </summary>
        /// <param name="nuget">Interface to the NuGet package manager</param>
        /// <param name="targetRoot">Target root directory</param>
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

            var depsRoot = targetRoot.CreateDirectory("deps");
            var depDir = depsRoot.CreateDirectory(reference.Uri.Host);

            var dlls = nuget.InstallPackage(reference.Uri.Host, depDir, "");
            return new HashSet<TargetRelativePath>(
                from path in dlls
                select new TargetRelativePath(
                    Path.Combine(targetRoot.GetRelativePath(depDir),
                                 ((string) path).TrimStart('\\'))));
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