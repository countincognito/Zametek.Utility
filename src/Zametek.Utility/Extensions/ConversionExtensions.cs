using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Zametek.Utility
{
    public static class ConversionExtensions
    {
        public static bool IsDataContract(this Type input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            var attributes = input.GetTypeInfo().GetCustomAttributes(typeof(DataContractAttribute), false);
            return attributes.Any();
        }

        public static bool IsDataContract<T>(this T input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            return input.GetType().IsDataContract();
        }

        public static bool CanSerialize(this Type input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            return input.IsDataContract() || input.GetTypeInfo().IsSerializable;
        }

        public static bool CanSerialize<T>(this T input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            return input.GetType().CanSerialize();
        }

        public static void ThrowIfCannotSerialize(this Type input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (!input.CanSerialize())
            {
                throw new InvalidOperationException($"Type {input.FullName} cannot be serialized or deserialized.");
            }
        }

        public static void ThrowIfCannotSerialize<T>(this T input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            input.GetType().ThrowIfCannotSerialize();
        }

        public static byte[] StringToByteArray(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            return Encoding.UTF8.GetBytes(input);
        }

        public static string ByteArrayToString(this byte[] input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return Encoding.UTF8.GetString(input);
        }

        public static byte[] Base64StringToByteArray(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            return Convert.FromBase64String(input);
        }

        public static string ByteArrayToBase64String(this byte[] input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            return Convert.ToBase64String(input);
        }

        public static byte[] ObjectToByteArray<T>(this T input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            input.ThrowIfCannotSerialize();
            string json = JsonConvert.SerializeObject(input);
            return json.StringToByteArray();
        }

        public static T ByteArrayToObject<T>(this byte[] input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            typeof(T).ThrowIfCannotSerialize();
            string json = input.ByteArrayToString();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T CloneObject<T>(this T input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            input.ThrowIfCannotSerialize();
            return input.ObjectToByteArray().ByteArrayToObject<T>();
        }

        public static string ObjectToBase64String<T>(this T input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            input.ThrowIfCannotSerialize();
            return input.ObjectToByteArray().ByteArrayToBase64String();
        }

        public static T Base64StringToObject<T>(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(nameof(input));
            }
            typeof(T).ThrowIfCannotSerialize();
            return input.Base64StringToByteArray().ByteArrayToObject<T>();
        }
    }
}
