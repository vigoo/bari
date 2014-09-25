using Bari.Core.Generic;

namespace Bari.Plugins.InnoSetup.Tools
{
    public interface IInnoSetupCompiler
    {
        void Compile(SuiteRelativePath scriptPath, TargetRelativePath targetPath, string version);
    }
}