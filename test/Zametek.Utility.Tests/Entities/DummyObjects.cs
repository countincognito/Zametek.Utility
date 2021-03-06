﻿using System;
using System.Runtime.Serialization;

namespace Zametek.Utility.Tests
{
    public class DummyObject
    {
        public int IntegerValue { get; set; }

        public double DoubleValue { get; set; }

        public string StringValue { get; set; }
    }

    [DataContract]
    public class DummyObjectDataContract
    {
        [DataMember]
        public int IntegerValue { get; set; }

        [DataMember]
        public double DoubleValue { get; set; }

        [DataMember]
        public string StringValue { get; set; }
    }

    [Serializable]
    public class DummyObjectSerializable
    {
        public int IntegerValue { get; set; }

        public double DoubleValue { get; set; }

        public string StringValue { get; set; }
    }
}
