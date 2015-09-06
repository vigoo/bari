using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Bari.Core.Model;

namespace Bari.Core.Build.MergingTag
{
    public class ProjectBuilderTag: DescriptionTag, IEquatable<ProjectBuilderTag>
    {
        private readonly ISet<Project> projects;

        public ProjectBuilderTag(string description, IEnumerable<Project> projects)
            : base(description)
        {
            Contract.Requires(projects != null);

            this.projects = new HashSet<Project>(projects);
        }

        public bool Equals(ProjectBuilderTag other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return projects.SetEquals(other.projects) && base.Equals(other);
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
            return projects.Aggregate(11, (n, prj) => n ^= prj.GetHashCode()) ^ base.GetHashCode();
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