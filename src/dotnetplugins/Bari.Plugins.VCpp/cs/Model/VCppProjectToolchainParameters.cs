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
            return new VCppProjectToolchainParameters(parent);
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
                    case PlatformToolSet.VS2022:
                        return "v143";
                    case PlatformToolSet.VS2019:
                        return "v142"; 
                    case PlatformToolSet.VS2017:
                        return "v141"; 
                    case PlatformToolSet.VS2015:
                        return "v140"; 
                    case PlatformToolSet.VS2013: 
                        return "v120";                 
                    case PlatformToolSet.VS2012: 
                    default: 
                        return "v110"; 
                }
            }
        }

        public VCppProjectToolchainParameters(VCppProjectToolchainParameters parent = null):
            base(parent)
        {
            
        }
    }
}

