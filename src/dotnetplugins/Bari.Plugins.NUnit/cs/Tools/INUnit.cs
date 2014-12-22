using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Plugins.NUnit.Tools
{
    public interface INUnit
    {
        bool RunTests(IEnumerable<TargetRelativePath> testAssemblies);
    }
}