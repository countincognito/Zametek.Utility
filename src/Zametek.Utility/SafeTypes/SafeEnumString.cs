using System;

namespace Zametek.Utility
{
    public abstract class SafeEnumString<T>
       : SafeEnumBase<string>, IEquatable<T>, IComparable<T>, IComparable where T : SafeEnumString<T>
    {
        public SafeEnumString(string value)
            : base(value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
        }

        public static implicit operator string(SafeEnumString<T> other) => other?.Value;

        public virtual bool Equals(T other) =>
            other is null ? false : string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object other) =>
            other is null ? false : string.Equals(Value, other.ToString(), StringComparison.OrdinalIgnoreCase);

        public static bool operator ==(SafeEnumString<T> left, SafeEnumString<T> right) =>
            left is null ? right is null : left.Equals(right);

        public static bool operator !=(SafeEnumString<T> left, SafeEnumString<T> right) =>
            left is null ? !(right is null) : !left.Equals(right);

        public static bool operator <(SafeEnumString<T> left, SafeEnumString<T> right) =>
            left is null ? !(right is null) : left.CompareTo(right) < 0;

        public static bool operator <=(SafeEnumString<T> left, SafeEnumString<T> right) =>
            left is null || left.CompareTo(right) <= 0;

        public static bool operator >(SafeEnumString<T> left, SafeEnumString<T> right) =>
            !(left is null) && left.CompareTo(right) > 0;

        public static bool operator >=(SafeEnumString<T> left, SafeEnumString<T> right) =>
            left is null ? right is null : left.CompareTo(right) >= 0;

        public virtual int CompareTo(T other) =>
            other is null ? -1 : string.Compare(Value, other.ToString(), StringComparison.OrdinalIgnoreCase);

        public virtual int CompareTo(object other) => CompareTo(other as T);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }
}
