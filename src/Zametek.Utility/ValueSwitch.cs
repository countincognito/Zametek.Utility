using System;

namespace Zametek.Utility
{
    public static class ValueSwitch
    {
        public static ValueSwitch<T> ValueSwitchOn<T>(this T value)
            where T : struct
        {
            return new ValueSwitch<T>(value);
        }
    }

    public class ValueSwitch<T>
        where T : struct
    {
        private readonly T m_SourceValue;
        private bool m_Handled = false;

        internal ValueSwitch(T value)
        {
            m_SourceValue = value;
        }

        public ValueSwitch<T> Case(T value, Action<T> action)
        {
            if (!m_Handled)
            {
                if (value.Equals(m_SourceValue))
                {
                    action(m_SourceValue);
                    m_Handled = true;
                }
            }
            return this;
        }

        public void Default(Action<T> action)
        {
            if (!m_Handled)
            {
                action(m_SourceValue);
            }
        }
    }
}