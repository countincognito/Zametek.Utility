using AutoFixture;
using FluentAssertions;
using System;
using Xunit;

namespace Zametek.Utility.Tests
{
    public class ConversionExtensionsTests
    {
        [Fact]
        public void ConversionExtensions_GivenIsDataContract_WhenInputIsDataContract_ThenIsTrue()
        {
            new DummyObject().IsDataContract().Should().BeFalse();
            new DummyObjectDataContract().IsDataContract().Should().BeTrue();
            new DummyObjectSerializable().IsDataContract().Should().BeFalse();

            typeof(DummyObject).IsDataContract().Should().BeFalse();
            typeof(DummyObjectDataContract).IsDataContract().Should().BeTrue();
            typeof(DummyObjectSerializable).IsDataContract().Should().BeFalse();
        }

        [Fact]
        public void ConversionExtensions_GivenCanSerialize_WhenInputCanBeSerialized_ThenIsTrue()
        {
            new DummyObject().CanSerialize().Should().BeFalse();
            new DummyObjectDataContract().CanSerialize().Should().BeTrue();
            new DummyObjectSerializable().CanSerialize().Should().BeTrue();

            typeof(DummyObject).CanSerialize().Should().BeFalse();
            typeof(DummyObjectDataContract).CanSerialize().Should().BeTrue();
            typeof(DummyObjectSerializable).CanSerialize().Should().BeTrue();
        }

        [Fact]
        public void ConversionExtensions_GivenThrowIfCannotSerialize_WhenInputCanBeSerialized_ThenThrowsNoException()
        {
            Action act;

            act = () => new DummyObject().ThrowIfCannotSerialize();
            act.Should().Throw<InvalidOperationException>();

            act = () => new DummyObjectDataContract().ThrowIfCannotSerialize();
            act.Should().NotThrow();

            act = () => new DummyObjectSerializable().ThrowIfCannotSerialize();
            act.Should().NotThrow();


            act = () => typeof(DummyObject).ThrowIfCannotSerialize();
            act.Should().Throw<InvalidOperationException>();

            act = () => typeof(DummyObjectDataContract).ThrowIfCannotSerialize();
            act.Should().NotThrow();

            act = () => typeof(DummyObjectSerializable).ThrowIfCannotSerialize();
            act.Should().NotThrow();
        }

        [Fact]
        public void ConversionExtensions_GivenObjectToByteArray_WhenOutputCanBeSerialized_ThenCorrectArrayCreated()
        {
            var fixture = new Fixture();
            var input = fixture
                .Build<DummyObjectSerializable>()
                .Create();

            byte[] array = input.ObjectToByteArray();

            array.Should().NotBeNullOrEmpty();

            var output = array.ByteArrayToObject<DummyObjectSerializable>();

            output.Should().NotBe(input);
            output.Should().BeEquivalentTo(input);
        }

        [Fact]
        public void ConversionExtensions_GivenObjectToByteArray_WhenOutputCannotBeSerialized_ThenThrowsInvalidOperationException()
        {
            var fixture = new Fixture();
            var input = fixture
                .Build<DummyObjectSerializable>()
                .Create();

            byte[] array = input.ObjectToByteArray();

            array.Should().NotBeNullOrEmpty();

            Action act = () => array.ByteArrayToObject<DummyObject>();
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ConversionExtensions_GivenObjectToBase64String_WhenOutputCanBeSerialized_ThenCorrectStringCreated()
        {
            var fixture = new Fixture();
            var input = fixture
                .Build<DummyObjectSerializable>()
                .Create();

            string base64 = input.ObjectToBase64String();

            base64.Should().NotBeNullOrEmpty();

            var output = base64.Base64StringToObject<DummyObjectSerializable>();

            output.Should().NotBe(input);
            output.Should().BeEquivalentTo(input);
        }

        [Fact]
        public void ConversionExtensions_GivenObjectToBase64String_WhenOutputCannotBeSerialized_ThenThrowsInvalidOperationException()
        {
            var fixture = new Fixture();
            var input = fixture
                .Build<DummyObjectSerializable>()
                .Create();

            string base64 = input.ObjectToBase64String();

            base64.Should().NotBeNullOrEmpty();

            Action act = () => base64.Base64StringToObject<DummyObject>();
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ConversionExtensions_GivenCloneObject_WhenOutputCanBeSerialized_ThenDeepCloneCreated()
        {
            var fixture = new Fixture();
            var input = fixture
                .Build<DummyObjectSerializable>()
                .Create();

            var output = input.CloneObject();
            output.Should().NotBe(input);
            output.Should().BeEquivalentTo(input);
        }
    }
}
