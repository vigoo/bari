using System;

namespace Bari.Core.Model
{
    public class PostProcessorId : IEquatable<PostProcessorId>
    {
        private readonly string id;

        public string AsString { get { return id; }}

        public PostProcessorId(string id)
        {
            this.id = id;
        }

        public static implicit operator PostProcessorId(string value)
        {
            return new PostProcessorId(value);
        }

        public override string ToString()
        {
            return string.Format("<{0}>", id);
        }

        public bool Equals(PostProcessorId other)
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
            return Equals((PostProcessorId) obj);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static bool operator ==(PostProcessorId left, PostProcessorId right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PostProcessorId left, PostProcessorId right)
        {
            return !Equals(left, right);
        }
    }
}