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

        protected override IEnumerable<SourceSet> GetSourceSets(Project project)
        {
            return new[] {project.GetSourceSet("fs")};
        }

        private static readonly ISet<string> ignoredExtensions = new HashSet<string>
            {
                ".fsproj",
                ".fsproj.user"
            };

        protected override ISet<string> IgnoredExtensions
        {
            get { return ignoredExtensions; }
        }
    }
}