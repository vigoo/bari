using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Csharp.Model;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;

namespace Bari.Plugins.Csharp.VisualStudio.CsprojSections
{
    /// <summary>
    /// .csproj section listing all the embedded resources
    /// </summary>
    public class EmbeddedResourcesSection : SourceItemsSectionBase
    {
        /// <summary>
        /// Initializes the class
        /// </summary>
        /// <param name="suite">Active suite</param>
        public EmbeddedResourcesSection(Suite suite)
            : base(suite)
        {
        }

        /// <summary>
        /// Gets the source sets to include 
        /// </summary>
        /// <param name="project">The project to get its source sets</param>
        /// <returns>Returns an enumeration of source sets, all belonging to the given project</returns>
        protected override IEnumerable<ISourceSet> GetSourceSets(Project project)
        {
            return new[] { project.GetSourceSet("resources") };
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
            var relativePath = ToProjectRelativePath(project, file, "resources");

            if (relativePath.StartsWith("wpf" + Path.DirectorySeparatorChar))
                return "Resource";
            else
                return "EmbeddedResource";
        }

        protected override string GetLogicalPath(Project project, SuiteRelativePath file, SourceSetType sourceSetType)
        {
            var path = base.GetLogicalPath(project, file, sourceSetType);
            if (path.StartsWith("wpf" + Path.DirectorySeparatorChar))
                return path.Substring(4).Replace(Path.DirectorySeparatorChar, '/');
            else
                return PrefixWithRootNamespace(project, PrefixNumericComponents(path)).Replace(Path.DirectorySeparatorChar, '.');
        }

        private string PrefixNumericComponents(string path)
        {
            return String.Join(Path.DirectorySeparatorChar.ToString(),
                path.Split(Path.DirectorySeparatorChar)
                    .Select(part => part.Length > 0 && char.IsDigit(part[0]) ? "_" + part : part));
        }

        private string PrefixWithRootNamespace(Project project, string path)
        {
            CsharpProjectParameters parameters = project.GetInheritableParameters<CsharpProjectParameters, CsharpProjectParametersDef>("csharp");
            parameters.FillProjectSpecificMissingInfo(project);

            return parameters.RootNamespace + "." + path;
        }

        protected override void WriteAdditionalOptions(XmlWriter writer, Project project, SuiteRelativePath suiteRelativePath)
        {
            base.WriteAdditionalOptions(writer, project, suiteRelativePath);

            var relativePath = ToProjectRelativePath(project, suiteRelativePath, "resources");

            if (relativePath.StartsWith("wpf" + Path.DirectorySeparatorChar))
                relativePath = GetLogicalPath(project, suiteRelativePath, "resources");
            else
                relativePath = Path.Combine("_Resources", relativePath);

            writer.WriteElementString("Link", relativePath);
        }


        private static readonly ISet<string> ignoredExtensions = new HashSet<string>
            {
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
            get { return "cs"; }
        }
    }
}