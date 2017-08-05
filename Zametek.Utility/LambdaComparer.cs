using System;
using System.Collections.Generic;

namespace Zametek.Utility
{
    public class LambdaComparer<T>
        : IComparer<T>
    {
        private readonly Func<T, T, int> m_LambdaComparer;

        public LambdaComparer(Func<T, T, int> lambdaComparer)
        {
            m_LambdaComparer = lambdaComparer ?? throw new ArgumentNullException(nameof(lambdaComparer));
        }

        public int Compare(T x, T y)
        {
            return m_LambdaComparer(x, y);
        }
    }
}
