using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Plugins.InnoSetup.Tools
{
    public interface IInnoSetupCompiler
    {
        void Compile(SuiteRelativePath scriptPath, TargetRelativePath targetPath, string version, Goal targetGoal);
    }
}