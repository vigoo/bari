using System;
using System.IO;
using Bari.Core.UI;

namespace Bari.Plugins.VsCore.Tools.Versions
{
    public class MSBuild40x86: MSBuild
    {
        public MSBuild40x86(IParameters parameters) :
            base(parameters, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v4.0.30319\"))
        {
        }
    }
    public class MSBuild40x64 : MSBuild
    {
        public MSBuild40x64(IParameters parameters) :
            base(parameters, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework64\v4.0.30319\"))
        {
        }
    }
    public class MSBuildVS2013: MSBuild
    {
        public MSBuildVS2013(IParameters parameters) :
            base(parameters, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"MSBuild\12.0\Bin\"))
        {
        }
    }
    public class MSBuildInPath: MSBuild
    {
        public MSBuildInPath(IParameters parameters) :
            base(parameters, "")
        {
        }
    }
}