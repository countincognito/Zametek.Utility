using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Zametek.Utility
{
    [DataContract]
    public class TrackingContext
    {
        private static readonly object s_Lock = new object();
        private readonly IDictionary<string, string> m_ExtraHeaders;

        static TrackingContext()
        {
            FullName = typeof(TrackingContext).FullName;
        }

        public TrackingContext(
            Guid callChainId,
            DateTime originatorUtcTimestamp,
            IDictionary<string, string> extraHeaders)
        {
            m_ExtraHeaders = extraHeaders ?? throw new ArgumentNullException(nameof(extraHeaders));
            CallChainId = callChainId;
            OriginatorUtcTimestamp = originatorUtcTimestamp;
            ExtraHeaders = new Dictionary<string, string>(m_ExtraHeaders);
        }

        public static string FullName
        {
            get;
        }

        public static TrackingContext Current
        {
            get
            {
                lock (s_Lock)
                {
                    return AmbientContext.GetData<TrackingContext>();
                }
            }
            private set
            {
                lock (s_Lock)
                {
                    AmbientContext.SetData(value);
                }
            }
        }

        [DataMember]
        public Guid CallChainId
        {
            get;
        }

        [DataMember]
        public DateTime OriginatorUtcTimestamp
        {
            get;
        }

        [DataMember]
        public IReadOnlyDictionary<string, string> ExtraHeaders
        {
            get;
        }

        public static void NewCurrentIfEmpty()
        {
            NewCurrentIfEmpty(new Dictionary<string, string>());
        }

        public static void NewCurrentIfEmpty(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }
            lock (s_Lock)
            {
                TrackingContext tc = Current;
                if (tc == null)
                {
                    NewCurrent(headers);
                }
            }
        }

        public static byte[] Serialize(TrackingContext trackingContext)
        {
            if (trackingContext == null)
            {
                throw new ArgumentNullException(nameof(trackingContext));
            }

            return trackingContext.ObjectToByteArray();
        }

        public static TrackingContext DeSerialize(byte[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            return array.ByteArrayToObject<TrackingContext>();
        }

        /// <summary>
        /// Clears the current TrackingContext. Use with caution.
        /// </summary>
        public static void ClearCurrent()
        {
            lock (s_Lock)
            {
                AmbientContext.Clear<TrackingContext>();
            }
        }

        /// <summary>
        /// Sets the instance to the current TrackingContext. Use with caution.
        /// </summary>
        public void SetAsCurrent()
        {
            lock (s_Lock)
            {
                Current = this;
            }
        }

        /// <summary>
        /// Replaces the current TrackingContext. Use with caution.
        /// </summary>
        public static void NewCurrent()
        {
            NewCurrent(new Dictionary<string, string>());
        }

        /// <summary>
        /// Replaces the current TrackingContext. Use with caution.
        /// </summary>
        /// <param name="headers">Key-Value pairs to be included in the new current TrackingContext.</param>
        public static void NewCurrent(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }
            lock (s_Lock)
            {
                Current = Create(headers);
            }
        }

        private static Guid NewInstanceId()
        {
            return Guid.NewGuid();
        }

        private static TrackingContext Create(IDictionary<string, string> headers)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }
            return new TrackingContext(NewInstanceId(), DateTime.UtcNow, headers);
        }
    }
}
