using System.Collections.Generic;
using Bari.Core.Model;
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
        protected override IEnumerable<SourceSet> GetSourceSets(Project project)
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
    }
}