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

            ConversionExtensions.IsDataContract(typeof(DummyObject)).Should().BeFalse();
            ConversionExtensions.IsDataContract(typeof(DummyObjectDataContract)).Should().BeTrue();
            ConversionExtensions.IsDataContract(typeof(DummyObjectSerializable)).Should().BeFalse();
        }

        [Fact]
        public void ConversionExtensions_GivenCanSerialize_WhenInputCanBeSerialized_ThenIsTrue()
        {
            new DummyObject().CanSerialize().Should().BeFalse();
            new DummyObjectDataContract().CanSerialize().Should().BeTrue();
            new DummyObjectSerializable().CanSerialize().Should().BeTrue();

            ConversionExtensions.CanSerialize(typeof(DummyObject)).Should().BeFalse();
            ConversionExtensions.CanSerialize(typeof(DummyObjectDataContract)).Should().BeTrue();
            ConversionExtensions.CanSerialize(typeof(DummyObjectSerializable)).Should().BeTrue();
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


            act = () => ConversionExtensions.ThrowIfCannotSerialize(typeof(DummyObject));
            act.Should().Throw<InvalidOperationException>();

            act = () => ConversionExtensions.ThrowIfCannotSerialize(typeof(DummyObjectDataContract));
            act.Should().NotThrow();

            act = () => ConversionExtensions.ThrowIfCannotSerialize(typeof(DummyObjectSerializable));
            act.Should().NotThrow();
        }

        [Fact]
        public void ConversionExtensions_GivenObjectToByteArray_WhenObjectCanBeSerialized_ThenCorrectArrayCreated()
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
        public void ConversionExtensions_GivenObjectToBase64String_WhenObjectCanBeSerialized_ThenCorrectStringCreated()
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
        public void ConversionExtensions_GivenCloneObject_WhenObjectCanBeSerialized_ThenDeepCloneCreated()
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
