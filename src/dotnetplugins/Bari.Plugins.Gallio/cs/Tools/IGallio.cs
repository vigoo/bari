using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Plugins.Gallio.Tools
{
    public interface IGallio
    {
        bool RunTests(IEnumerable<TargetRelativePath> testAssemblies);
    }
}