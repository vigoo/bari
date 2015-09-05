using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectToolchainParametersDef : ProjectParametersPropertyDefs<VCppProjectToolchainParameters>
    {
        public VCppProjectToolchainParametersDef()
        {
            Define<PlatformToolSet>("PlatformToolSet");
        }

        public override VCppProjectToolchainParameters CreateDefault(Suite suite, VCppProjectToolchainParameters parent)
        {
            return new VCppProjectToolchainParameters();
        }
    }

    public class VCppProjectToolchainParameters : InheritableProjectParameters<VCppProjectToolchainParameters, VCppProjectToolchainParametersDef>
    {
        public PlatformToolSet PlatformToolSet
        {
            get { return Get<PlatformToolSet>("PlatformToolSet"); }
            set { Set("PlatformToolSet", value); }
        }

        public bool IsPlatformToolSetSpecified { get { return IsSpecified("PlatformToolSet"); } }

        public string PlatformToolSetAsString
        {
            get
            { 
                switch (PlatformToolSet)
                {                                    
                    case PlatformToolSet.VS2013: 
                        return "v120";                 
                    case PlatformToolSet.VS2012: 
                    default: 
                        return "v110"; 
                }
            }
        }
    }
}

