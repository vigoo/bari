using System;
using System.Collections.Generic;
using Bari.Core.Build.Cache;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// Copies the contents of a given project's <c>content</c> source set to the target directory
    /// </summary>
    [ShouldNotCache]
    public class ContentBuilder: BuilderBase<ContentBuilder>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof (ContentBuilder));

        private readonly Project project;
        private readonly ISourceSetFingerprintFactory fingerprintFactory;
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IFileSystemDirectory targetRoot;

        public ContentBuilder(Project project, ISourceSetFingerprintFactory fingerprintFactory, [SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.project = project;
            this.fingerprintFactory = fingerprintFactory;
            this.suiteRoot = suiteRoot;
            this.targetRoot = targetRoot;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public override IDependencies Dependencies
        {
            get
            {
                return new SourceSetDependencies(fingerprintFactory, project.GetSourceSet("content"));
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public override string Uid
        {
            get { return project.Module + "." + project.Name; }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public override ISet<TargetRelativePath> Run(IBuildContext context)
        {
            var contents = project.GetSourceSet("content");
            var contentsDir = project.RootDirectory.GetChildDirectory("content");

            var targetDir = targetRoot.GetChildDirectory(project.Module.Name, createIfMissing: true);
            var result = new HashSet<TargetRelativePath>();
            foreach (var sourcePath in contents.Files)
            {
                log.DebugFormat("Copying content {0}...", sourcePath);

                var relativePath = suiteRoot.GetRelativePathFrom(contentsDir, sourcePath);

                suiteRoot.CopyFile(sourcePath, targetDir, relativePath);

                result.Add(new TargetRelativePath(project.Module.Name, relativePath));
            }

            return result;
        }

        public override string ToString()
        {
            return String.Format("[{0}.{1}/content]", project.Module.Name, project.Name);
        }
    }
}