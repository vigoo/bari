using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectCLIParametersDef : ProjectParametersPropertyDefs<VCppProjectCLIParameters>
    {
        public VCppProjectCLIParametersDef()
        {
            Define<CppCliMode>("Mode");
        }

        public override VCppProjectCLIParameters CreateDefault(Suite suite, VCppProjectCLIParameters parent)
        {
            return new VCppProjectCLIParameters();
        }
    }

    public class VCppProjectCLIParameters : VCppProjectParametersBase<VCppProjectCLIParameters, VCppProjectCLIParametersDef>
    {
        public CppCliMode Mode
        {
            get { return Get<CppCliMode>("Mode"); }
            set { Set("Mode", value); }
        }

        public bool IsModeSpecified { get { return IsSpecified("Mode"); } }
    }
}