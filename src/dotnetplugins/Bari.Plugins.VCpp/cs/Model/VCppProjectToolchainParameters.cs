using System;
using Bari.Core.Model;

namespace Bari.Plugins.VCpp.Model
{
    public class VCppProjectToolchainParameters: IProjectParameters
    {
        private PlatformToolSet platformToolSet = PlatformToolSet.VS2012;

        public PlatformToolSet PlatformToolSet { get; set; }

        public string PlatformToolSetAsString
        {
            get
            { 
                switch (platformToolSet)
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

