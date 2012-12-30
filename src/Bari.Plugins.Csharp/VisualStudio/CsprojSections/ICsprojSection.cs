using System.Xml;
using Bari.Core.Model;

namespace Bari.Plugins.Csharp.VisualStudio.CsprojSections
{
    /// <summary>
    /// Generates a section to a .csproj file
    /// </summary>
    public interface ICsprojSection
    {
        /// <summary>
        /// Writes the section using an XML writer
        /// </summary>
        /// <param name="writer">XML writer to use</param>
        /// <param name="project">The project to generate .csproj for</param>
        /// <param name="context">Current .csproj generation context</param>
        void Write(XmlWriter writer, Project project, ICsprojGeneratorContext context);
    }
}