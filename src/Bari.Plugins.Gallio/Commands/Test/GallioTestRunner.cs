using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Commands.Test;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.Gallio.Tools;

namespace Bari.Plugins.Gallio.Commands.Test
{
    public class GallioTestRunner: ITestRunner
    {
        private readonly IGallio gallio;

        public GallioTestRunner(IGallio gallio)
        {
            this.gallio = gallio;
        }

        public void Run(IEnumerable<TestProject> projects, IEnumerable<TargetRelativePath> buildOutputs)
        {
            gallio.RunTests(buildOutputs.Where(
                path => ((string)path).EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}