using System;

namespace Zametek.Utility
{
    public abstract class SafeEnumBase<T>
    {
        public SafeEnumBase(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        public T Value { get; }
    }
}
