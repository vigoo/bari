using System.Collections.Generic;
using System.Linq;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.VisualStudio;
using Ninject.Syntax;

namespace Bari.Plugins.Csharp.Build
{
    /// <summary>
    /// Builder generating a Visual C# project file from a source code set
    /// 
    /// <para>Uses the <see cref="CsprojGenerator"/> class internally.</para>
    /// </summary>
    public class CsprojBuilder : IBuilder
    {
        private readonly IProjectGuidManagement projectGuidManagement;
        private readonly IResolutionRoot root;
        private readonly Project project;
        private readonly IFileSystemDirectory targetDir;
        private readonly ISet<IBuilder> referenceBuilders;

        /// <summary>
        /// Creates the builder
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to use</param>
        /// <param name="root">Path to resolve instances</param>
        /// <param name="project">The project for which the csproj file will be generated</param>
        /// <param name="targetDir">The target directory where the csproj file should be put</param>
        /// <param name="referenceBuilders">Project reference builders</param>
        public CsprojBuilder(IProjectGuidManagement projectGuidManagement, IResolutionRoot root, Project project, [TargetRoot] IFileSystemDirectory targetDir, IEnumerable<IBuilder> referenceBuilders)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.root = root;
            this.project = project;
            this.targetDir = targetDir;
            this.referenceBuilders = new HashSet<IBuilder>(referenceBuilders);
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get
            {
                if (project.HasNonEmptySourceSet("cs"))
                {
                    return new MultipleDependencies(
                        new IDependencies[]
                            {
                                new SourceSetDependencies(root, project.GetSourceSet("cs")),
                                new ProjectPropertiesDependencies(project, "Name", "Type")
                            }
                            .Concat(
                                from refBuilder in referenceBuilders
                                select new SubtaskDependency(refBuilder)));
                }
                else
                {
                    return new NoDependencies();
                }
            }
        }

        /// <summary>
        /// Gets an unique identifier which can be used to identify cached results
        /// </summary>
        public string Uid
        {
            get { return project.Module.Name + "." + project.Name; }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <param name="context"> </param>
        /// <returns>Returns a set of generated files, in suite relative paths</returns>
        public ISet<TargetRelativePath> Run(IBuildContext context)
        {            
            var csprojPath = project.Name + ".csproj";
            using (var csproj = targetDir.CreateTextFile(csprojPath))
            {
                var references = new HashSet<TargetRelativePath>();
                foreach (var refBuilder in referenceBuilders)
                {
                    var builderResults = context.GetResults(refBuilder);
                    references.UnionWith(builderResults);
                }

                var generator = new CsprojGenerator(projectGuidManagement, "..", project, references, csproj); // TODO: path to suite root should not be hard coded
                generator.Generate();
            }

            return new HashSet<TargetRelativePath>(
                new[]
                    {
                        new TargetRelativePath(csprojPath)
                    });
        }
    }
}