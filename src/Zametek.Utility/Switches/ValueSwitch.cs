using System;

namespace Zametek.Utility
{
    public static class ValueSwitch
    {
        public static ValueSwitch<TSource> ValueSwitchOn<TSource>(this TSource value)
            where TSource : IEquatable<TSource>
        {
            return new ValueSwitch<TSource>(value);
        }
    }

    public class ValueSwitch<TSource>
        where TSource : IEquatable<TSource>
    {
        private readonly TSource m_Source;
        private bool m_Handled = false;

        internal ValueSwitch(TSource value)
        {
            m_Source = value;
        }

        public ValueSwitch<TSource> Case(TSource value, Action<TSource> action)
        {
            if (m_Source == null)
            {
                return this;
            }
            if (!m_Handled)
            {
                if (value.Equals(m_Source))
                {
                    if (action == null)
                    {
                        throw new ArgumentNullException(nameof(action));
                    }
                    action(m_Source);
                    m_Handled = true;
                }
            }
            return this;
        }

        public void Default(Action<TSource> action)
        {
            if (!m_Handled)
            {
                if (action == null)
                {
                    throw new ArgumentNullException(nameof(action));
                }
                action(m_Source);
            }
        }
    }
}