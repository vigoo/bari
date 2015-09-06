using System;
using System.Diagnostics.Contracts;

namespace Bari.Core.Build.MergingTag
{
    public class DescriptionTag: IMergingBuilderTag, IEquatable<DescriptionTag>
    {
        private readonly string description;

        public DescriptionTag(string description)
        {
            Contract.Requires(description != null);

            this.description = description;
        }

        public bool Equals(DescriptionTag other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(description, other.description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DescriptionTag) obj);
        }

        public override int GetHashCode()
        {
            return description.GetHashCode();
        }

        public static bool operator ==(DescriptionTag left, DescriptionTag right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DescriptionTag left, DescriptionTag right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return description;
        }
    }
}