using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace Zametek.Utility
{
    /// <summary>
    /// Provides a way to set contextual data that flows with the call and 
    /// async context of a test or invocation.
    /// </summary>
    public static class AmbientContext
    {
        #region Fields

        private static AsyncLocal<ConcurrentDictionary<Type, AsyncLocal<byte[]>>> s_State = new AsyncLocal<ConcurrentDictionary<Type, AsyncLocal<byte[]>>>();

        #endregion

        #region Properties

        private static ConcurrentDictionary<Type, AsyncLocal<byte[]>> State
        {
            get => s_State.Value;
            set => s_State.Value = value;
        }

        #endregion

        #region Private Members

        private static ConcurrentDictionary<Type, AsyncLocal<byte[]>> GetOrCreateState()
        {
            ConcurrentDictionary<Type, AsyncLocal<byte[]>> state = State;

            if (state == null)
            {
                state = new ConcurrentDictionary<Type, AsyncLocal<byte[]>>();
                State = state;
            }

            return state;
        }

        private static bool IsDataContract(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(DataContractAttribute), false);
            return attributes.Any();
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData<T>(T data) where T : class
        {
            Debug.Assert(IsDataContract(typeof(T)) || typeof(T).IsSerializable);

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ConcurrentDictionary<Type, AsyncLocal<byte[]>> state = GetOrCreateState();
            state.GetOrAdd(typeof(T), _ => new AsyncLocal<byte[]>()).Value = Serialize(data);
            State = state;
        }

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static T GetData<T>() where T : class
        {
            Debug.Assert(IsDataContract(typeof(T)) || typeof(T).IsSerializable);

            ConcurrentDictionary<Type, AsyncLocal<byte[]>> state = GetOrCreateState();

            if (state.TryGetValue(typeof(T), out AsyncLocal<byte[]> data))
            {
                if (data.Value == null)
                {
                    return null;
                }

                return DeSerialize<T>(data.Value);
            }

            return null;
        }

        public static void Clear<T>() where T : class
        {
            ConcurrentDictionary<Type, AsyncLocal<byte[]>> state = GetOrCreateState();
            state.TryRemove(typeof(T), out AsyncLocal<byte[]> data);
            State = state;
        }

        public static byte[] Serialize<T>(T obj) where T : class
        {
            Debug.Assert(IsDataContract(typeof(T)) || typeof(T).IsSerializable);

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            string json = JsonConvert.SerializeObject(obj);
            return Encoding.UTF8.GetBytes(json);
        }

        public static T DeSerialize<T>(byte[] array) where T : class
        {
            Debug.Assert(IsDataContract(typeof(T)) || typeof(T).IsSerializable);

            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            string json = Encoding.UTF8.GetString(array);
            return JsonConvert.DeserializeObject<T>(json);
        }

        #endregion
    }
}
