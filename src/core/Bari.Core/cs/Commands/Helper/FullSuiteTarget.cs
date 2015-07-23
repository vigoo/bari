using System.Collections.Generic;
using System.Linq;
using Bari.Core.Model;

namespace Bari.Core.Commands.Helper
{
    public class FullSuiteTarget: CommandTarget
    {
        private readonly Suite suite;

        public FullSuiteTarget(Suite suite)
        {
            this.suite = suite;
        }

        public override IEnumerable<Project> Projects
        {
            get
            {
                return from module in suite.Modules
                       from project in module.Projects
                       select project;
            }
        }

        public override IEnumerable<TestProject> TestProjects
        {
            get
            {
                return from module in suite.Modules
                       from project in module.TestProjects
                       select project;
            }
        }
    }
}