using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Exceptions;
using Bari.Core.Generic;
using Bari.Core.UI;

namespace Bari.Core.Model
{
    public class DefaultSuiteFactory: ISuiteFactory
    {
        private readonly string targetGoal;
        private readonly IFileSystemDirectory suiteRoot;

        public DefaultSuiteFactory(IParameters parameters, [SuiteRoot] IFileSystemDirectory suiteRoot)
        {
            targetGoal = parameters.Goal;
            this.suiteRoot = suiteRoot;
        }

        public Suite CreateSuite(ISet<Goal> goals)
        {
            if (goals.Count == 0)
            {
                goals.Add(Suite.DebugGoal);
                goals.Add(Suite.ReleaseGoal);
            }

            var activeGoal =
                goals.FirstOrDefault(g => g.Name.Equals(targetGoal, StringComparison.InvariantCultureIgnoreCase));
            if (activeGoal == null)
                throw new InvalidGoalException(targetGoal, goals);

            return new Suite(suiteRoot, goals, activeGoal);
        }
    }
}