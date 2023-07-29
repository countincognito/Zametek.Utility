using System;
using System.Collections.Generic;

namespace Zametek.Utility
{
    public class LambdaEqualityComparer<T>
        : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> m_LambdaEqualityComparer;
        private readonly Func<T, int> m_LambdaHash;

        public LambdaEqualityComparer(Func<T, T, bool> lambdaEqualityComparer) :
            this(lambdaEqualityComparer, x => x.GetHashCode())
        {
        }

        public LambdaEqualityComparer(
            Func<T, T, bool> lambdaEqualityComparer,
            Func<T, int> lambdaHash)
        {
            m_LambdaEqualityComparer = lambdaEqualityComparer ?? throw new ArgumentNullException(nameof(lambdaEqualityComparer));
            m_LambdaHash = lambdaHash ?? throw new ArgumentNullException(nameof(lambdaHash));
        }

        public bool Equals(T x, T y)
        {
            return m_LambdaEqualityComparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return m_LambdaHash(obj);
        }
    }
}
