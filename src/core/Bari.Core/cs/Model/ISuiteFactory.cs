using System.Collections.Generic;

namespace Bari.Core.Model
{
    public interface ISuiteFactory
    {
        /// <summary>
        /// Creates a new suite
        /// </summary>
        /// <param name="goals">Goal definitions pre-read from the suite specification</param>
        /// <param name="defaultGoal">Default goal to be used if no target goal was specified</param>
        /// <returns>Returns the new suite</returns>
        Suite CreateSuite(ISet<Goal> goals, Goal defaultGoal);
    }
}