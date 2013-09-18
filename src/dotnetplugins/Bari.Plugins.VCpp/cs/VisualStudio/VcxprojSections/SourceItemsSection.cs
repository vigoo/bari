using System.Collections.Generic;
using System.IO;
using Bari.Core.Model;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;

namespace Bari.Plugins.VCpp.VisualStudio.VcxprojSections
{
    public class SourceItemsSection : SourceItemsSectionBase
    {
        public SourceItemsSection(Suite suite)
            : base(suite)
        {
        }

        /// <summary>
        /// Gets the element name for a given compilation item.
        /// 
        /// <para>The default implementation always returns <c>Compile</c></para>
        /// </summary>
        /// <param name="file">File name from the source set</param>
        /// <returns>Returns a valid XML element name</returns>
        protected override string GetElementNameFor(string file)
        {
            var ext = Path.GetExtension(file).ToLowerInvariant();
            if (ext == ".h" || ext == ".hpp" || ext == ".h++")
                return "CLInclude";
            else
                return "CLCompile";
        }

        /// <summary>
        /// Gets the source sets to include 
        /// </summary>
        /// <param name="project">The project to get its source sets</param>
        /// <returns>Returns an enumeration of source sets, all belonging to the given project</returns>
        protected override IEnumerable<SourceSet> GetSourceSets(Project project)
        {
            return new[] { project.GetSourceSet("cpp") };
        }

        private static readonly ISet<string> ignoredExtensions = new HashSet<string>
            {
                ".vcxproj",
                ".vcxproj.user"
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
            get { return "cpp"; }
        }
    }
}