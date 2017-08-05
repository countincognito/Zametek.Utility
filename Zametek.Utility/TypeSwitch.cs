using System;
using System.Reflection;

namespace Zametek.Utility
{
    public static class TypeSwitch
    {
        public static TypeSwitch<TSource> TypeSwitchOn<TSource>(this TSource value)
        {
            return new TypeSwitch<TSource>(value);
        }
    }

    public class TypeSwitch<TSource>
    {
        private readonly TSource m_Source;
        private bool m_Handled = false;

        internal TypeSwitch(TSource value)
        {
            m_Source = value;
        }

        public TypeSwitch<TSource> Case<TTarget>(Action<TTarget> action)
            where TTarget : TSource
        {
            if (m_Source == null)
            {
                return this;
            }
            if (!m_Handled)
            {
                TypeInfo sourceTypeInfo = m_Source.GetType().GetTypeInfo();
                TypeInfo targetTypeInfo = typeof(TTarget).GetTypeInfo();
                if (targetTypeInfo.IsAssignableFrom(sourceTypeInfo))
                {
                    action((TTarget)m_Source);
                    m_Handled = true;
                }
            }
            return this;
        }

        public void Default(Action<TSource> action)
        {
            if (!m_Handled)
            {
                action(m_Source);
            }
        }
    }
}