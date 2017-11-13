using System;
using System.Xml;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectManifestParametersDef : ProjectParametersPropertyDefs<VCppProjectManifestParameters>
    {
        public VCppProjectManifestParametersDef()
        {
            Define<bool>("EmbedManifest");
            Define<bool>("GenerateManifest");
            Define<string>("TypeLibraryFile");
            Define<string>("ComponentFileName");
        }

        public override VCppProjectManifestParameters CreateDefault(Suite suite, VCppProjectManifestParameters parent)
        {
            return new VCppProjectManifestParameters(parent);
        }
    }

    public class VCppProjectManifestParameters : InheritableProjectParameters<VCppProjectManifestParameters, VCppProjectManifestParametersDef>
    {
        public bool EmbedManifest
        {
            get { return Get<bool>("EmbedManifest"); }
            set { Set("EmbedManifest", value); }
        }

        public bool IsEmbedManifestSpecified { get { return IsSpecified("EmbedManifest"); } }

        public bool GenerateManifest
        {
            get { return Get<bool>("GenerateManifest"); }
            set { Set("GenerateManifest", value); }
        }

        public bool IsGenerateManifestSpecified { get { return IsSpecified("GenerateManifest"); } }

        public string TypeLibraryFile
        {
            get { return Get<string>("TypeLibraryFile"); }
            set { Set("TypeLibraryFile", value); }
        }

        public bool IsTypeLibraryFileSpecified { get { return IsSpecified("TypeLibraryFile"); } }

        public string ComponentFileName
        {
            get { return Get<string>("ComponentFileName"); }
            set { Set("ComponentFileName", value); }
        }

        public bool IsComponentFileNameSpecified { get { return IsSpecified("ComponentFileName"); } }

        public VCppProjectManifestParameters(VCppProjectManifestParameters parent = null)
            : base(parent)
        {
        }

        public void FillProjectSpecificMissingInfo(Project project)
        {
            if (project.HasParameters("midl"))
            {
                var midlParams = project.GetParameters<VCppProjectMIDLParameters>("midl");
                TypeLibraryFile = midlParams.TypeLibraryName;         
            }
        }

        public void ToVcxprojProperties(XmlWriter writer)
        {
            if (IsTypeLibraryFileSpecified && !String.IsNullOrEmpty(TypeLibraryFile))
                writer.WriteElementString("TypeLibraryFile", TypeLibraryFile);
            if (IsComponentFileNameSpecified && !String.IsNullOrEmpty(ComponentFileName))
                writer.WriteElementString("ComponentFileName", ComponentFileName);
        }
    }
}