using System;

namespace Bari.Core.Build.MergingTag
{
    public class NoTag: IMergingBuilderTag, IEquatable<NoTag>
    {
        public bool Equals(NoTag other)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NoTag) obj);
        }

        public override int GetHashCode()
        {
            return 1231078;
        }

        public static bool operator ==(NoTag left, NoTag right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NoTag left, NoTag right)
        {
            return !Equals(left, right);
        }
    }
}