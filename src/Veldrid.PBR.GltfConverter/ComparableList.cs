using System;
using System.Collections.Generic;

namespace Veldrid.PBR
{
    public class ComparableList<T> : List<T>, IEquatable<ComparableList<T>>
    {
        public ComparableList()
        {
            
        }

        public ComparableList(IEnumerable<T> values)
        {
            AddRange(values);
        }
        public bool Equals(IList<T> other)
        {
            if (other == null)
                return false;
            if (Count != other.Count)
                return false;
            for (var index = 0; index < this.Count; index++)
            {
                if (!_comparer.Equals(this[index], other[index]))
                    return false;
            }

            return true;
        }

        bool IEquatable<ComparableList<T>>.Equals(ComparableList<T> other)
        {
            return Equals(other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ComparableList<T>) obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 0;
            for (var index = 0; index < this.Count; index++)
            {
                hashCode = (hashCode * 397) ^ this[index].GetHashCode();
            }

            return hashCode;
        }

        public static bool operator ==(ComparableList<T> left, ComparableList<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ComparableList<T> left, ComparableList<T> right)
        {
            return !Equals(left, right);
        }

        private static readonly IEqualityComparer<T> _comparer = EqualityComparer<T>.Default;

    }
}