using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.VisualStudio;
using Ninject;
using Ninject.Extensions.ChildKernel;
using Ninject.Syntax;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Builder for generating Visual Studio solution files from a set of projects.
    /// </summary>
    public class SlnBuilder : IBuilder
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IResolutionRoot root;
        private readonly IList<Project> projects;
        private readonly IFileSystemDirectory targetDir;

        private readonly ISet<IBuilder> projectBuilders;
        private readonly MultipleDependencies projectDependencies;

        /// <summary>
        /// Creates the builder
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to use</param>
        /// <param name="root">Path to create instances</param>
        /// <param name="projects">Projects to be included in the solution</param>
        /// <param name="targetDir">The target directory where the sln file should be put</param>
        public SlnBuilder(IProjectGuidManagement projectGuidManagement, IResolutionRoot root, IEnumerable<Project> projects, [TargetRoot] IFileSystemDirectory targetDir)
        {
            Contract.Requires(projectGuidManagement != null);
            Contract.Requires(root != null);
            Contract.Requires(projects != null);
            Contract.Requires(targetDir != null);

            this.projectGuidManagement = projectGuidManagement;
            this.root = root;
            this.projects = projects.ToList();
            this.targetDir = targetDir;

            projectBuilders = new HashSet<IBuilder>(
                from project in this.projects
                select CreateProjectBuilder(project)
                into builder
                where builder != null
                select builder
                );

            projectDependencies = new MultipleDependencies(
                from builder in projectBuilders
                select new SubtaskDependency(builder));
        }

        private IBuilder CreateProjectBuilder(Project project)
        {
            if (project.HasNonEmptySourceSet("cs"))
            {
                var childKernel = new ChildKernel(root);
                childKernel.Bind<Project>().ToConstant(project);

                return childKernel.Get<CsprojBuilder>();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get { return projectDependencies; }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <returns>Returns a set of generated files, in target relative paths</returns>
        public ISet<TargetRelativePath> Run()
        {
            const string slnPath = "generated.sln";
            var result = new HashSet<TargetRelativePath>();

            foreach (var projectBuilder in projectBuilders)
            {
                var projectOutputs = projectBuilder.Run();
                result.UnionWith(projectOutputs);
            }

            using (var sln = targetDir.CreateTextFile(slnPath))
            {
                var generator = new SlnGenerator(projectGuidManagement, projects, sln);
                generator.Generate();
            }

            result.Add(new TargetRelativePath(slnPath));
            return result;
        }
    }
}