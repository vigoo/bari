using System.Xml;
using Bari.Core.Model;
using Bari.Plugins.CodeContracts.Model;
using Bari.Plugins.VsCore.VisualStudio;
using Bari.Plugins.VsCore.VisualStudio.ProjectSections;

namespace Bari.Plugins.CodeContracts.VisualStudio.CsprojSections
{
    public class CodeContractsSection : MSBuildProjectSectionBase
    {
        public CodeContractsSection(Suite suite)
            : base(suite)
        {
        }

        public override void Write(XmlWriter writer, Project project, IMSBuildProjectGeneratorContext context)
        {
            if (project.HasParameters("contracts"))
            {
                writer.WriteStartElement("PropertyGroup");

                var parameters = project.GetParameters<ContractsProjectParameters>("contracts");
                parameters.ToCsprojProperties(writer);

                writer.WriteEndElement();
            }
        }
    }
}