using System;
using System.Collections.Generic;
using Bari.Core.Commands;
using Bari.Core.Commands.Helper;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.AddonSupport.Model;
using Bari.Plugins.VsCore.VisualStudio.SolutionItems;

namespace Bari.Plugins.AddonSupport.VisualStudio.SolutionItems
{
    class AddonSupportSolutionItemProvider : ISolutionItemProvider
    {
        private readonly IFileSystemDirectory targetRoot;
        private readonly Suite suite;
        private readonly ICommand currentCommand;
        private readonly ICommandTargetParser targetParser;

        public AddonSupportSolutionItemProvider([TargetRoot] IFileSystemDirectory targetRoot, Suite suite, [Current] ICommand currentCommand, ICommandTargetParser targetParser)
        {
            this.targetRoot = targetRoot;
            this.suite = suite;
            this.currentCommand = currentCommand;
            this.targetParser = targetParser;
        }

        public IEnumerable<TargetRelativePath> GetItems(string solutionName)
        {
            var path = GenerateAddonSupportFile(solutionName);

            return new[] { path };
        }

        private TargetRelativePath GenerateAddonSupportFile(string solutionName)
        {
            string contents = AddonSupportData();
            var path = new TargetRelativePath("", solutionName + ".yaml");
            bool writeFile = true;

            if (targetRoot.Exists(path.RelativePath))
            {
                using (var reader = targetRoot.ReadTextFile(path.RelativePath))
                {
                    string existingContents = reader.ReadToEnd();
                    writeFile = existingContents != contents;
                }
            }

            if (writeFile)
            {
                using (var writer = targetRoot.CreateTextFile(path.RelativePath))
                    writer.WriteLine(contents);
            }

            return path;
        }

        private string AddonSupportData()
        {
            var data = new AddonSupportSolutionItemData(targetParser, currentCommand as IHasBuildTarget, suite.ActiveGoal);

            return String.Format(@"---
bari-path: {0}
goal: {1}
target: {2}
startup-path: {3}
", data.BariPath, data.Goal, data.Target, data.StartupPath);
        }

    }
}
