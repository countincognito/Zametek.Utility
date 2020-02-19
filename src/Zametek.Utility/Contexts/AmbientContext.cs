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

        private static readonly AsyncLocal<ConcurrentDictionary<Type, AsyncLocal<byte[]>>> s_State = new AsyncLocal<ConcurrentDictionary<Type, AsyncLocal<byte[]>>>();

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

        #endregion

        #region Public Members

        /// <summary>
        /// Stores a given object and associates it with the specified name.
        /// </summary>
        /// <param name="name">The name with which to associate the new item in the call context.</param>
        /// <param name="data">The object to store in the call context.</param>
        public static void SetData<T>(T data) where T : class
        {
            Debug.Assert(ConversionExtensions.CanSerialize(typeof(T)));

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            ConcurrentDictionary<Type, AsyncLocal<byte[]>> state = GetOrCreateState();
            state.GetOrAdd(typeof(T), _ => new AsyncLocal<byte[]>()).Value = data.ObjectToByteArray();
            State = state;
        }

        /// <summary>
        /// Retrieves an object with the specified name from the <see cref="CallContext"/>.
        /// </summary>
        /// <param name="name">The name of the item in the call context.</param>
        /// <returns>The object in the call context associated with the specified name, or <see langword="null"/> if not found.</returns>
        public static T GetData<T>() where T : class
        {
            Debug.Assert(ConversionExtensions.CanSerialize(typeof(T)));

            ConcurrentDictionary<Type, AsyncLocal<byte[]>> state = GetOrCreateState();

            if (state.TryGetValue(typeof(T), out AsyncLocal<byte[]> data))
            {
                if (data.Value == null)
                {
                    return null;
                }

                return data.Value.ByteArrayToObject<T>();
            }

            return null;
        }

        public static void Clear<T>() where T : class
        {
            ConcurrentDictionary<Type, AsyncLocal<byte[]>> state = GetOrCreateState();
            state.TryRemove(typeof(T), out _);
            State = state;
        }

        #endregion
    }
}
