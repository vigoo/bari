using System.Collections.Generic;
using System.IO;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.VCpp.Model;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;

namespace Bari.Plugins.VCpp.VisualStudio.VcxprojSections
{
    public class SourceItemsSection : SourceItemsSectionBase
    {
        private readonly IFileSystemDirectory suiteRoot;
        private readonly IProjectPlatformManagement platformManagement;

        public SourceItemsSection(Suite suite, IProjectPlatformManagement platformManagement)
            : base(suite)
        {
            this.platformManagement = platformManagement;
            suiteRoot = suite.SuiteRoot;
        }

        /// <summary>
        /// Gets the element name for a given compilation item.
        /// 
        /// <para>The default implementation always returns <c>Compile</c></para>
        /// </summary>
        /// <param name="file">File name from the source set</param>
        /// <returns>Returns a valid XML element name</returns>
        protected override string GetElementNameFor(Project project, string file)
        {
            var ext = Path.GetExtension(file).ToLowerInvariant();
            if (ext == ".h" || ext == ".hpp" || ext == ".h++")
                return "CLInclude";
            else if (ext == ".rc")
                return "ResourceCompile";
            else
                return "CLCompile";
        }

        /// <summary>
        /// Gets the source sets to include 
        /// </summary>
        /// <param name="project">The project to get its source sets</param>
        /// <returns>Returns an enumeration of source sets, all belonging to the given project</returns>
        protected override IEnumerable<ISourceSet> GetSourceSets(Project project)
        {
            return new[] { project.GetSourceSet("cpp").FilterCppSourceSet(project.RootDirectory.GetChildDirectory("cpp"), suiteRoot) };
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

        /// <summary>
        /// Provides the ability to add extra content to a given project source file
        /// </summary>
        /// <param name="writer">The project file writer</param>
        /// <param name="project">Project model</param>
        /// <param name="suiteRelativePath">Suite relative path of the source item</param>
        protected override void WriteAdditionalOptions(XmlWriter writer, Project project, SuiteRelativePath suiteRelativePath)
        {
            base.WriteAdditionalOptions(writer, project, suiteRelativePath);

            var projectRelativePath = ToProjectRelativePath(project, suiteRelativePath, ProjectSourceSetName);

            if (projectRelativePath == "stdafx.cpp")
            {
                var platform = platformManagement.GetDefaultPlatform(project);

                writer.WriteStartElement("PrecompiledHeader");
                writer.WriteAttributeString("Condition", string.Format("'$(Configuration)|$(Platform)' == 'Bari|{0}' ", platform));
                writer.WriteString("Create");
                writer.WriteEndElement();
            }
        }
    }
}