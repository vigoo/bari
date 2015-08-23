using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Model;

namespace Bari.Core.Build.MergingTag
{
    public class ProjectBuilderTag: IMergingBuilderTag, IEquatable<ProjectBuilderTag>
    {
        private readonly ISet<Project> projects;

        public ProjectBuilderTag(IEnumerable<Project> projects)
        {
            Contract.Requires(projects != null);

            this.projects = new HashSet<Project>(projects);
        }

        public bool Equals(ProjectBuilderTag other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return projects.SetEquals(other.projects);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProjectBuilderTag) obj);
        }

        public override int GetHashCode()
        {
            return projects.Aggregate(11, (n, prj) => n ^= prj.GetHashCode());
        }

        public static bool operator ==(ProjectBuilderTag left, ProjectBuilderTag right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ProjectBuilderTag left, ProjectBuilderTag right)
        {
            return !Equals(left, right);
        }
    }
}