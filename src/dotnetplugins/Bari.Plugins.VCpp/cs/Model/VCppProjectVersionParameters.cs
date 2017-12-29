using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectVersionParametersDef : ProjectParametersPropertyDefs<VCppProjectVersionParameters>
    {
        public VCppProjectVersionParametersDef()
        {
            Define<bool>("VersionSupport");
        }

        public override VCppProjectVersionParameters CreateDefault(Suite suite, VCppProjectVersionParameters parent)
        {
            return new VCppProjectVersionParameters();
        }
    }

    public class VCppProjectVersionParameters : InheritableProjectParameters<VCppProjectVersionParameters, VCppProjectVersionParametersDef>
    {
        public bool VersionSupport
        {
            get { return Get<bool>("VersionSupport"); }
            set { Set("VersionSupport", value); }
        }

        public bool IsVersionSupportSpecified { get { return IsSpecified("VersionSupport"); } }
    }
}

