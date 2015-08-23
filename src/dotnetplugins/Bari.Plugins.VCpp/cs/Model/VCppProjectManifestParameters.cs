using System;
using System.Xml;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectManifestParameters: IProjectParameters
    {
        public bool EmbedManifest { get; set; } 
        public bool GenerateManifest { get; set; } 
        public string TypeLibraryFile { get; set; }
        public string ComponentFileName { get; set; }

        public VCppProjectManifestParameters(Suite suite)
        {
        }

        public void FillProjectSpecificMissingInfo(Project project)
        {
            if (project.HasParameters("midl"))
            {
                var midlParams = project.GetParameters<VCppProjectMIDLParameters>("midl");
                TypeLibraryFile = midlParams.TypeLibraryName;         
            }

            ComponentFileName = "$(TargetFileName)";
        }

        public void ToVcxprojProperties(XmlWriter writer)
        {
            if (!String.IsNullOrEmpty(TypeLibraryFile))
                writer.WriteElementString("TypeLibraryFile", TypeLibraryFile);
            if (!String.IsNullOrEmpty(ComponentFileName))
                writer.WriteElementString("ComponentFileName", ComponentFileName);
        }
    }
}