using System;

namespace Bari.Core.Model
{
    public class PackagerId : IEquatable<PackagerId>
    {
        private readonly string id;

        public string AsString { get { return id; }}

        public PackagerId(string id)
        {
            this.id = id;
        }

        public static implicit operator PackagerId(string value)
        {
            return new PackagerId(value);
        }

        public override string ToString()
        {
            return string.Format("<{0}>", id);
        }

        public bool Equals(PackagerId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(id, other.id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PackagerId)obj);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static bool operator ==(PackagerId left, PackagerId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PackagerId left, PackagerId right)
        {
            return !Equals(left, right);
        }
    }
}