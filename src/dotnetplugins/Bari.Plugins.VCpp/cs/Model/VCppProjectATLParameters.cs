using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectATLParametersDef : ProjectParametersPropertyDefs<VCppProjectATLParameters>
    {
        public VCppProjectATLParametersDef()
        {
            Define<UseOfATL>("UseOfATL");
        }

        public override VCppProjectATLParameters CreateDefault(Suite suite, VCppProjectATLParameters parent)
        {
            return new VCppProjectATLParameters();
        }
    }

    public class VCppProjectATLParameters: VCppProjectParametersBase<VCppProjectATLParameters, VCppProjectATLParametersDef>
    {
        public UseOfATL UseOfATL
        {
            get { return Get<UseOfATL>("UseOfATL"); }
            set { Set("UseOfATL", value); }
        }

        public bool IsUseOfATLSpecified { get { return IsSpecified("UseOfATL"); } }
    }
}