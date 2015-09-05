using System.Collections.Generic;
using Bari.Core.Model.Parameters;

namespace Bari.Core.Model
{
    public class Tests: IProjectParameters
    {
        private ISet<string> enabledTestRunners = new HashSet<string>();
        public ISet<string> EnabledTestRunners
        {
            get { return enabledTestRunners; }
        }

        public void EnableAllTestRunners()
        {
            enabledTestRunners = new HashSet<string>();
        }

        public void EnableTestRunners(IEnumerable<string> runners)
        {
            enabledTestRunners = new HashSet<string>(runners);
        }

        public bool IsRunnerEnabled(string runner)
        {
            if (enabledTestRunners.Count == 0)
                return true;
            else
                return enabledTestRunners.Contains(runner);
        }
    }
}