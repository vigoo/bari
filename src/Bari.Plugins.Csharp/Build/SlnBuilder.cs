using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.VisualStudio;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Builder for generating Visual Studio solution files from a set of projects.
    /// </summary>
    public class SlnBuilder : IBuilder
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IFileSystemDirectory targetDir;
        private readonly IList<Project> projects;
        private readonly ISet<IBuilder> projectBuilders;
        private readonly IDependencies projectDependencies;

        /// <summary>
        /// Creates the builder
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to use</param>
        /// <param name="projects">The projects to be included to the solution</param>
        /// <param name="projectBuilders">Projects builders to be used as dependencies</param>
        /// <param name="targetDir">The target directory where the sln file should be put</param>
        public SlnBuilder(IProjectGuidManagement projectGuidManagement, IEnumerable<Project> projects, IEnumerable<IBuilder> projectBuilders, [TargetRoot] IFileSystemDirectory targetDir)
        {
            Contract.Requires(projectGuidManagement != null);
            Contract.Requires(projects != null);
            Contract.Requires(projectBuilders != null);
            Contract.Requires(targetDir != null);

            this.projectGuidManagement = projectGuidManagement;
            this.projects = projects.ToList();
            this.projectBuilders = new HashSet<IBuilder>(projectBuilders);
            this.targetDir = targetDir;

            if (this.projectBuilders.Count == 1)
            {
                projectDependencies = new SubtaskDependency(this.projectBuilders.First());
            }
            else
            {
                projectDependencies = new MultipleDependencies(
                    from builder in this.projectBuilders
                    select new SubtaskDependency(builder));
            }
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return projectDependencies; }
        }

        public string Uid
        {
            get
            {
                return MD5.Encode(string.Join(",",
                                   from project in projects
                                   let module = project.Module
                                   let fullName = module + "." + project.Name
                                   select fullName));
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

            return new HashSet<TargetRelativePath> { new TargetRelativePath(slnPath) };
        }
    }
}