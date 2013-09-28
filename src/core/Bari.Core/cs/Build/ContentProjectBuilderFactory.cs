using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Build
{
    /// <summary>
    /// A <see cref="IProjectBuilderFactory"/> implementation that creates <see cref="ContentBuilder"/>
    /// instances for projects having a source set named <c>content</c>.
    /// </summary>
    public class ContentProjectBuilderFactory: IProjectBuilderFactory
    {
        private readonly ISourceSetFingerprintFactory fingerprintFactory;
        private readonly IFileSystemDirectory targetRoot;
        private readonly IFileSystemDirectory suiteRoot;

        public ContentProjectBuilderFactory(ISourceSetFingerprintFactory fingerprintFactory, [SuiteRoot] IFileSystemDirectory suiteRoot, [TargetRoot] IFileSystemDirectory targetRoot)
        {
            this.fingerprintFactory = fingerprintFactory;
            this.suiteRoot = suiteRoot;
            this.targetRoot = targetRoot;
        }

        /// <summary>
        /// Adds the builders (<see cref="IBuilder"/>) to the given build context which process
        /// the given set of projects (<see cref="Project"/>)
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="projects">Projects to be built</param>
        public IBuilder AddToContext(IBuildContext context, IEnumerable<Project> projects)
        {
            IBuilder[] builders = projects.Where(prj => prj.HasNonEmptySourceSet("content"))
                                          .Select(prj => (IBuilder)new ContentBuilder(prj, fingerprintFactory, suiteRoot, targetRoot)).ToArray();

            return builders.Merge();
        }
    }
}