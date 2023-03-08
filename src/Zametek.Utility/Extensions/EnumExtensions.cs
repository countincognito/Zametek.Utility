using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Zametek.Utility
{
    public static class EnumExtensions
    {
        public static void ValidateValue<T>(this object value, string argumentName)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (string.IsNullOrWhiteSpace(argumentName))
            {
                throw new ArgumentNullException(nameof(argumentName));
            }

            Type type = typeof(T);

            if (!type.GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException($@"Type {type.FullName} is not an Enum.");
            }

            int intValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);

            if (!Enum.IsDefined(type, intValue))
            {
                throw new InvalidOperationException($@"Value {intValue} for Enum {type.FullName} is not defined");
            }
        }

        public static string GetDescription(this Enum value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            FieldInfo field = value.GetType().GetField(value.ToString());

            return field.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute attribute
                ? attribute.Description
                : value.ToString();
        }

        public static T GetValueFromDescription<T>(this string description)
        {
            Type type = typeof(T);

            if (!type.GetTypeInfo().IsEnum)
            {
                throw new InvalidOperationException($@"Type {type.FullName} is not an Enum.");
            }

            foreach (var field in type.GetFields())
            {
                if (field.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    if (attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase))
                    {
                        return (T)field.GetValue(null);
                    }
                }
                else
                {
                    if (field.Name.Equals(description, StringComparison.OrdinalIgnoreCase))
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            return default;
        }
    }
}
