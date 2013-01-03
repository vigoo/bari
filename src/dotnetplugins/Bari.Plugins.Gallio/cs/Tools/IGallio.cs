using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Plugins.Gallio.Tools
{
    public interface IGallio
    {
        void RunTests(IEnumerable<TargetRelativePath> testAssemblies);
    }
}