using System;
using System.Collections.Generic;
using System.Linq;

namespace Bari.Core.Model
{
    public class Goal : IEquatable<Goal>
    {
        private readonly string name;
        private readonly ISet<Goal> incorporatedGoals;

        public string Name
        {
            get { return name; }
        }

        public IEnumerable<Goal> IncorporatedGoals
        {
            get { return incorporatedGoals; }
        }

        public Goal(string name)
        {
            this.name = name;
            incorporatedGoals = new HashSet<Goal>();
        }

        public Goal(string name, IEnumerable<Goal> incorporatedGoals)
        {
            this.name = name;
            this.incorporatedGoals = new HashSet<Goal>(incorporatedGoals);
        }

        public bool Has(string desiredName)
        {
            if (this.name.Equals(desiredName, StringComparison.InvariantCultureIgnoreCase))
                return true;
            else
                return incorporatedGoals.Any(g => g.Has(desiredName));
        }

        public bool Equals(Goal other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(name, other.name, StringComparison.InvariantCultureIgnoreCase) &&
                   incorporatedGoals.SetEquals(other.incorporatedGoals);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Goal) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (name.ToLowerInvariant().GetHashCode()*397) ^
                       incorporatedGoals.Aggregate(11, (h, g) => h ^ g.GetHashCode());
            }
        }

        public static bool operator ==(Goal left, Goal right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Goal left, Goal right)
        {
            return !Equals(left, right);
        }
    }
}