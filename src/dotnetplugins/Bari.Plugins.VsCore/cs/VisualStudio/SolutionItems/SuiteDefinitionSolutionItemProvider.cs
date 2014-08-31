using System.Collections.Generic;
using Bari.Core.Generic;

namespace Bari.Plugins.VsCore.VisualStudio.SolutionItems
{
    public class SuiteDefinitionSolutionItemProvider : ISolutionItemProvider
    {
        public IEnumerable<TargetRelativePath> GetItems(string solutionName)
        {
            return new[]
                {
                    new TargetRelativePath("..", "suite.yaml")
                };
        }
    }
}