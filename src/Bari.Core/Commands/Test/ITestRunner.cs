using System.Collections.Generic;
using Bari.Core.Generic;
using Bari.Core.Model;

namespace Bari.Core.Commands.Test
{
    public interface ITestRunner
    {
        void Run(IEnumerable<TestProject> projects, IEnumerable<TargetRelativePath> buildOutputs);
    }
}