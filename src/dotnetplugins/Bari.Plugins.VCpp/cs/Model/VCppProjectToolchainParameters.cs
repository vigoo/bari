using System;
using Bari.Core.Model;
using Bari.Core.Model.Parameters;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectToolchainParameters: IProjectParameters
    {
        public PlatformToolSet PlatformToolSet { get; set; }

        public VCppProjectToolchainParameters()
        {
            PlatformToolSet = PlatformToolSet.VS2012;
        }

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

