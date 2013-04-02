using System;
using System.Collections.Generic;
using System.Linq;
using Bari.Core.Model;

namespace Bari.Core.Exceptions
{
    [Serializable]
    public class InvalidGoalException: Exception
    {
        private readonly string goalName;
        private readonly ISet<Goal> availableGoals;

        public InvalidGoalException(string goalName, ISet<Goal> availableGoals)
        {
            this.goalName = goalName;
            this.availableGoals = availableGoals;
        }

        public override string ToString()
        {
            return string.Format("Goal '{0}' is invalid. Available goals: {1}",
                                 goalName,
                                 String.Join(", ", availableGoals.Select(g => g.Name)));
        }
    }
}