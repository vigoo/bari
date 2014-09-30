using System;

namespace Bari.Core.Model
{
    public class SourceSetType : IEquatable<SourceSetType>
    {
        private readonly string type;

        public string AsString { get { return type; }}

        public SourceSetType(string type)
        {
            this.type = type;
        }

        public static implicit operator SourceSetType(string value)
        {
            return new SourceSetType(value);
        }

        public override string ToString()
        {
            return type;
        }

        public bool Equals(SourceSetType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(type, other.type);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SourceSetType)obj);
        }

        public override int GetHashCode()
        {
            return type.GetHashCode();
        }

        public static bool operator ==(SourceSetType left, SourceSetType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SourceSetType left, SourceSetType right)
        {
            return !Equals(left, right);
        }
    }
}