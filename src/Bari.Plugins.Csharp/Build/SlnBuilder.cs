using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.VisualStudio;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Builder for generating Visual Studio soluton files from a set of projects.
    /// </summary>
    public class SlnBuilder: IBuilder
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IList<Project> projects;
        private readonly IFileSystemDirectory targetDir;

        /// <summary>
        /// Creates the builder
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to use</param>
        /// <param name="projects">Projects to be included in the solution</param>
        /// <param name="targetDir">The target directory where the sln file should be put</param>
        public SlnBuilder(IProjectGuidManagement projectGuidManagement, IEnumerable<Project> projects, [TargetRoot] IFileSystemDirectory targetDir)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.projects = projects.ToList();
            this.targetDir = targetDir;

            //
            //            foreach (var project in projects)
            //            {
            //                if (project.HasNonEmptySourceSet("cs"))
            //                {
            //                    var childKernel = new ChildKernel(root);
            //                    childKernel.Bind<Project>().ToConstant(project);
            //
            //                    var builder = childKernel.Get<CsprojBuilder>();
            //                    var outputs = builder.Run();
            //
            //                    foreach (var outputPath in outputs)
            //                    {
            //                        log.InfoFormat("Generated output for project {0}: {1}", project.Name, outputPath);
            //                    }
            //                }
            //            }

        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get
            {
                // TODO
                return new NoDependencies(); 
            }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run()
        {
            const string slnPath = "generated.sln";

            using (var sln = targetDir.CreateTextFile(slnPath))
            {
                var generator = new SlnGenerator(projectGuidManagement, projects, sln);
                generator.Generate();
            }

            return new HashSet<TargetRelativePath>(
                new[] { new TargetRelativePath(slnPath) });
        }
    }
}