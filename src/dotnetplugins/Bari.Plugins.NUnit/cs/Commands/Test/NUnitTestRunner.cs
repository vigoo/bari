using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Commands.Test;
using Bari.Core.Generic;
using Bari.Core.Model;
using Bari.Plugins.NUnit.Tools;

namespace Bari.Plugins.NUnit.Commands.Test
{
    public class NUnitTestRunner: ITestRunner
    {
        private readonly INUnit nunit;

        public NUnitTestRunner(INUnit nunit)
        {
            this.nunit = nunit;
        }

        public string Name
        {
            get { return "nunit"; }
        }

        public bool Run(IEnumerable<TestProject> projects, IEnumerable<TargetRelativePath> buildOutputs)
        {
            return nunit.RunTests(buildOutputs.Where(
                path => ((string)path).EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}