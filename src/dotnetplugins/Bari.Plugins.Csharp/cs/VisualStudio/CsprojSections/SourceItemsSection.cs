using System.Collections.Generic;
using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;

namespace Bari.Plugins.Csharp.VisualStudio.CsprojSections
{
    /// <summary>
    /// .csproj section listing all the source files
    /// </summary>
    public class SourceItemsSection: SourceItemsSectionBase
    {
        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="suite">Active suite</param>
        public SourceItemsSection(Suite suite) : base(suite)
        {
        }

        protected override IEnumerable<SourceSet> GetSourceSets(Project project)
        {
            return new[] {project.GetSourceSet("cs")};
        }

        private static readonly ISet<string> ignoredExtensions = new HashSet<string>
            {
                ".csproj",
                ".csproj.user"
            };

        protected override ISet<string> IgnoredExtensions
        {
            get { return ignoredExtensions; }
        }
    }
}