using System.Collections.Generic;
using System.IO;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Fsharp.Model;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;

namespace Bari.Plugins.Fsharp.VisualStudio.FsprojSections
{
    public class SourceItemsSection: SourceItemsSectionBase
    {
        public SourceItemsSection(Suite suite) : base(suite)
        {
        }

        /// <summary>
        /// Gets the source sets to include 
        /// </summary>
        /// <param name="project">The project to get its source sets</param>
        /// <returns>Returns an enumeration of source sets, all belonging to the given project</returns>
        protected override IEnumerable<ISourceSet> GetSourceSets(Project project)
        {
            return new[] {project.GetSourceSet("fs")};
        }

        private static readonly ISet<string> ignoredExtensions = new HashSet<string>
            {
                ".fsproj",
                ".fsproj.user"
            };

        /// <summary>
        /// Gets a set of filename postfixes to be ignored when generating the source references
        /// </summary>
        protected override ISet<string> IgnoredExtensions
        {
            get { return ignoredExtensions; }
        }

        /// <summary>
        /// Source set name where the project file is placed
        /// </summary>
        protected override string ProjectSourceSetName
        {
            get { return "fs"; }
        }

        /// <summary>
        /// Orders the given set of source files if necessary
        /// </summary>
        /// <param name="project">Project being processed</param>
        /// <param name="files">Input sequence</param>
        /// <returns>Output sequence</returns>
        protected override IEnumerable<SuiteRelativePath> OrderSourceFiles(Project project, IEnumerable<SuiteRelativePath> files)
        {
            if (project.HasParameters("order"))
            {
                var order = project.GetParameters<FsharpFileOrder>("order");
                var remaining = new HashSet<SuiteRelativePath>(files);

                foreach (string file in order.OrderedFiles)
                {
                    var path = new SuiteRelativePath(
                        Path.Combine(Suite.SuiteRoot.GetRelativePath(project.RootDirectory), "fs", file));

                    if (remaining.Contains(path))
                    {
                        remaining.Remove(path);
                        yield return path;
                    }
                }

                foreach (var path in remaining)
                    yield return path;
            }
            else
            {
                foreach (var file in base.OrderSourceFiles(project, files))
                    yield return file;
            }
        }
    }
}