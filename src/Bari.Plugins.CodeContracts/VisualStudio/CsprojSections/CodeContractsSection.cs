using System.Xml;
using Bari.Core.Model;
using Bari.Plugins.CodeContracts.Model;
using Bari.Plugins.Csharp.VisualStudio;
using Bari.Plugins.Csharp.VisualStudio.CsprojSections;

namespace Bari.Plugins.CodeContracts.VisualStudio.CsprojSections
{
    public class CodeContractsSection : CsprojSectionBase
    {
        public CodeContractsSection(Suite suite)
            : base(suite)
        {
        }

        public override void Write(XmlWriter writer, Project project, ICsprojGeneratorContext context)
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