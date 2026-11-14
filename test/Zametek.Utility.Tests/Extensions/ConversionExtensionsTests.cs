using AutoFixture;
using Shouldly;
using System;
using Xunit;

namespace Zametek.Utility.Tests
{
    public class ConversionExtensionsTests
    {
        [Fact]
        public void ConversionExtensions_GivenIsDataContract_WhenInputIsDataContract_ThenIsTrue()
        {
            new DummyObject().IsDataContract().ShouldBeFalse();
            new DummyObjectDataContract().IsDataContract().ShouldBeTrue();
            new DummyObjectSerializable().IsDataContract().ShouldBeFalse();

            typeof(DummyObject).IsDataContract().ShouldBeFalse();
            typeof(DummyObjectDataContract).IsDataContract().ShouldBeTrue();
            typeof(DummyObjectSerializable).IsDataContract().ShouldBeFalse();
        }

        [Fact]
        public void ConversionExtensions_GivenCanSerialize_WhenInputCanBeSerialized_ThenIsTrue()
        {
            new DummyObject().CanSerialize().ShouldBeFalse();
            new DummyObjectDataContract().CanSerialize().ShouldBeTrue();
            new DummyObjectSerializable().CanSerialize().ShouldBeTrue();

            typeof(DummyObject).CanSerialize().ShouldBeFalse();
            typeof(DummyObjectDataContract).CanSerialize().ShouldBeTrue();
            typeof(DummyObjectSerializable).CanSerialize().ShouldBeTrue();
        }

        [Fact]
        public void ConversionExtensions_GivenThrowIfCannotSerialize_WhenInputCanBeSerialized_ThenThrowsNoException()
        {
            Action act;

            act = () => new DummyObject().ThrowIfCannotSerialize();
            act.ShouldThrow<InvalidOperationException>();

            act = () => new DummyObjectDataContract().ThrowIfCannotSerialize();
            act.ShouldNotThrow();

            act = () => new DummyObjectSerializable().ThrowIfCannotSerialize();
            act.ShouldNotThrow();


            act = () => typeof(DummyObject).ThrowIfCannotSerialize();
            act.ShouldThrow<InvalidOperationException>();

            act = () => typeof(DummyObjectDataContract).ThrowIfCannotSerialize();
            act.ShouldNotThrow();

            act = () => typeof(DummyObjectSerializable).ThrowIfCannotSerialize();
            act.ShouldNotThrow();
        }

        [Fact]
        public void ConversionExtensions_GivenObjectToByteArray_WhenOutputCanBeSerialized_ThenCorrectArrayCreated()
        {
            var fixture = new Fixture();
            var input = fixture
                .Build<DummyObjectSerializable>()
                .Create();

            byte[] array = input.ObjectToByteArray();

            array.ShouldNotBeNull();
            array.ShouldNotBeEmpty();

            var output = array.ByteArrayToObject<DummyObjectSerializable>();

            output.ShouldNotBe(input);
            output.ShouldBeEquivalentTo(input);
        }

        [Fact]
        public void ConversionExtensions_GivenObjectToByteArray_WhenOutputCannotBeSerialized_ThenThrowsInvalidOperationException()
        {
            var fixture = new Fixture();
            var input = fixture
                .Build<DummyObjectSerializable>()
                .Create();

            byte[] array = input.ObjectToByteArray();

            array.ShouldNotBeNull();
            array.ShouldNotBeEmpty();

            Action act = () => array.ByteArrayToObject<DummyObject>();
            act.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void ConversionExtensions_GivenObjectToBase64String_WhenOutputCanBeSerialized_ThenCorrectStringCreated()
        {
            var fixture = new Fixture();
            var input = fixture
                .Build<DummyObjectSerializable>()
                .Create();

            string base64 = input.ObjectToBase64String();

            base64.ShouldNotBeNullOrEmpty();

            var output = base64.Base64StringToObject<DummyObjectSerializable>();

            output.ShouldNotBe(input);
            output.ShouldBeEquivalentTo(input);
        }

        [Fact]
        public void ConversionExtensions_GivenObjectToBase64String_WhenOutputCannotBeSerialized_ThenThrowsInvalidOperationException()
        {
            var fixture = new Fixture();
            var input = fixture
                .Build<DummyObjectSerializable>()
                .Create();

            string base64 = input.ObjectToBase64String();

            base64.ShouldNotBeNullOrEmpty();

            Action act = () => base64.Base64StringToObject<DummyObject>();
            act.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void ConversionExtensions_GivenCloneObject_WhenOutputCanBeSerialized_ThenDeepCloneCreated()
        {
            var fixture = new Fixture();
            var input = fixture
                .Build<DummyObjectSerializable>()
                .Create();

            var output = input.CloneObject();
            output.ShouldNotBe(input);
            output.ShouldBeEquivalentTo(input);
        }
    }
}
