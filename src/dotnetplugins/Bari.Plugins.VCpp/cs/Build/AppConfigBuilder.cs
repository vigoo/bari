using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VsCore.Exceptions;

namespace Bari.Plugins.VCpp.Build
{
    /// <summary>
    /// Application configuration file builder for <see cref="Bari.Plugins.VCpp.VisualStudio.CppSlnProject"/>
    /// </summary>
    public class AppConfigBuilder : BuilderBase<AppConfigBuilder>, IEquatable<AppConfigBuilder>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(AppConfigBuilder));
        private const string appConfigSourceSetName = "appconfig";

        private readonly Project project;
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IDependencies dependencies;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigBuilder"/> class.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="fingerprintFactory">The fingerprint factory</param>
        /// <param name="suiteRoot">Suite's root directory</param>
        /// <param name="targetRoot">Target root directory</param>
        public AppConfigBuilder(Project project, ISourceSetFingerprintFactory fingerprintFactory, [SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.project = project;
            this.suiteRoot = suiteRoot;
            this.targetRoot = targetRoot;
            dependencies = new SourceSetDependencies(fingerprintFactory, project.GetSourceSet(appConfigSourceSetName));
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public override IDependencies Dependencies
        {
            get
            {
                return dependencies;
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public override string Uid
        {
            get { return project.Module + "." + project.Name + ".config"; }
        }
       
        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <exception cref="TooManyAppConfigsException"></exception>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public override ISet<TargetRelativePath> Run(IBuildContext context)
        {
            if (project.HasNonEmptySourceSet(appConfigSourceSetName))
            {
                var configs = project.GetSourceSet("appconfig");

                if (configs.Files.Count() > 1)
                    throw new TooManyAppConfigsException(project);

                if (project.Type != ProjectType.Executable)
                    throw new InvalidSpecificationException(string.Format("Invalid project type ({0}) for {1}.{2} or unecessary application config file ({3})!",
                        project.Type, project.Module.Name, project.Name, ToString()));
                
                var configFile = configs.Files.FirstOrDefault();

                log.DebugFormat("Copying config {0}...", configFile);

                var targetDir = targetRoot.GetChildDirectory(project.RelativeTargetPath, createIfMissing: true);
                var relativePath = project.Name + ".exe.config";
                suiteRoot.CopyFile(configFile, targetDir, relativePath);
                return new HashSet<TargetRelativePath> { new TargetRelativePath(project.RelativeTargetPath, relativePath) };
            }
            else
            {
                return new HashSet<TargetRelativePath>();
            }
        }

        public override string ToString()
        {
            return String.Format("[{0}.{1}/appconfig]", project.Module.Name, project.Name);
        }

        public bool Equals(AppConfigBuilder other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(project, other.project);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AppConfigBuilder)obj);
        }

        public override int GetHashCode()
        {
            return (project != null ? project.GetHashCode() : 0);
        }

        public static bool operator ==(AppConfigBuilder left, AppConfigBuilder right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AppConfigBuilder left, AppConfigBuilder right)
        {
            return !Equals(left, right);
        }
    }
}