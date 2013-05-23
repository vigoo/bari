using System.Xml;
using Bari.Core.Model;

namespace Bari.Plugins.VsCore.VisualStudio.ProjectSections
{
    /// <summary>
    /// Generates a section to a .csproj or similar file
    /// </summary>
    public interface IMSBuildProjectSection
    {
        /// <summary>
        /// Writes the section using an XML writer
        /// </summary>
        /// <param name="writer">XML writer to use</param>
        /// <param name="project">The project to generate .csproj for</param>
        /// <param name="context">Current .csproj generation context</param>
        void Write(XmlWriter writer, Project project, IMSBuildProjectGeneratorContext context);
    }
}