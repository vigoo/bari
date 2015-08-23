using System;
using System.Collections.Generic;

namespace Bari.Core.Model.Parameters
{
    public interface IPropertyValue
    {
        bool IsSpecified { get; }
        object Value { get; }
    }

    public class PropertyValue<T> : IPropertyValue, IEquatable<PropertyValue<T>>
    {
        private readonly bool isSpecified;
        private readonly T value;

        public bool IsSpecified
        {
            get { return isSpecified; }
        }

        public T Value
        {
            get
            {
                if (!isSpecified)
                    throw new InvalidOperationException("Property value is not defined");
                return value;
            }
        }

        object IPropertyValue.Value
        {
            get { return Value; }
        }

        public PropertyValue(T value)
        {
            isSpecified = true;
            this.value = value;
        }

        public PropertyValue()
        {
            isSpecified = false;
        } 

        public static PropertyValue<T> Unspecified { get { return new PropertyValue<T>();} }

        public bool Equals(PropertyValue<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return isSpecified.Equals(other.isSpecified) && EqualityComparer<T>.Default.Equals(value, other.value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PropertyValue<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (isSpecified.GetHashCode()*397) ^ EqualityComparer<T>.Default.GetHashCode(value);
            }
        }

        public static bool operator ==(PropertyValue<T> left, PropertyValue<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PropertyValue<T> left, PropertyValue<T> right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            if (isSpecified)
                return string.Format("Value: {0}", value);
            else
                return "Unspecified";
        }
    }
}