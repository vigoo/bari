using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Plugins.VsCore.VisualStudio.SolutionItems
{
    public interface ISolutionItemProvider
    {
        IEnumerable<TargetRelativePath> GetItems(string solutionName);
    }
}