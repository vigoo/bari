using System.Collections.Generic;
using Bari.Core.Build;
using Bari.Core.Build.Dependencies;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.VisualStudio;
using Ninject;
using Ninject.Parameters;
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

        /// <summary>
        /// Creates the builder
        /// </summary>
        /// <param name="projectGuidManagement">The project-guid mapper to use</param>
        /// <param name="root">Path to resolve instances</param>
        /// <param name="project">The project for which the csproj file will be generated</param>
        /// <param name="targetDir">The target directory where the csproj file should be put</param>
        public CsprojBuilder(IProjectGuidManagement projectGuidManagement, IResolutionRoot root, Project project, [TargetRoot] IFileSystemDirectory targetDir)
        {
            this.projectGuidManagement = projectGuidManagement;
            this.root = root;
            this.project = project;
            this.targetDir = targetDir;
        }

        /// <summary>
        /// Dependencies required for running this builder
        /// </summary>
        public IDependencies Dependencies
        {
            get
            {
                if (project.HasSourceSet("cs"))
                {
                    return root.Get<SourceSetDependencies>(new Parameter("sourceSet", project.GetSourceSet("cs"), false));                    
                }
                else
                {
                    return new NoDependencies();
                }
            }
        }

        /// <summary>
        /// Runs this builder
        /// </summary>
        /// <returns>Returns a set of generated files, in suite relative paths</returns>
        public ISet<TargetRelativePath> Run()
        {
            var csprojPath = project.Name + ".csproj";
            using (var csproj = targetDir.CreateTextFile(csprojPath))
            {
                var generator = new CsprojGenerator(projectGuidManagement, "..", project, csproj); // TODO: path to suite root should not be hard coded
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