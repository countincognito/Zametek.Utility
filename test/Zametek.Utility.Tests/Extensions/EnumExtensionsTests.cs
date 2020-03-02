using FluentAssertions;
using System;
using System.ComponentModel;
using Xunit;

namespace Zametek.Utility.Tests
{
    public class EnumExtensionsTests
    {
        [Fact]
        public void EnumExtensions_GivenValidateValue_WhenEnumIsValid_ThenThrowsNoException()
        {
            DummyEnum dummyEnum = DummyEnum.StateA;
            Action act = () => dummyEnum.ValidateValue<DummyEnum>(nameof(dummyEnum));
            act.Should().NotThrow();
        }

        [Fact]
        public void EnumExtensions_GivenValidateValue_WhenEnumIsInvalid_ThenThrowsInvalidOperationException()
        {
            DummyEnum dummyEnum = (DummyEnum)100;
            Action act = () => dummyEnum.ValidateValue<DummyEnum>(nameof(dummyEnum));
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void EnumExtensions_GivenGetDescription_WhenEnumHasDescription_ThenDescriptionReturned()
        {
            DummyEnum.StateA.GetDescription().Should().Be(@"A");
            DummyEnum.StateB.GetDescription().Should().Be(@"B");
            DummyEnum.StateC.GetDescription().Should().Be(@"C");
        }

        [Fact]
        public void EnumExtensions_GivenGetDescription_WhenEnumHasNoDescription_ThenNameOfEnumValueReturned()
        {
            DummyEnum.StateD.GetDescription().Should().Be(nameof(DummyEnum.StateD));
            DummyEnum.StateE.GetDescription().Should().Be(nameof(DummyEnum.StateE));
            DummyEnum.StateF.GetDescription().Should().Be(nameof(DummyEnum.StateF));
        }

        [Fact]
        public void EnumExtensions_GivenGetValueFromDescription_WhenEnumHasDescription_ThenDescriptionReturned()
        {
            @"A".GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateA);
            @"B".GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateB);
            @"C".GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateC);
        }

        [Fact]
        public void EnumExtensions_GivenGetValueFromDescription_WhenSearchingRandomDescription_ThenDefaultReturned()
        {
            Guid.NewGuid().ToFlatString().GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateA);
            Guid.NewGuid().ToFlatString().GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateA);
            Guid.NewGuid().ToFlatString().GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateA);
        }

        [Fact]
        public void EnumExtensions_GivenGetValueFromDescription_WhenSearchingNameOfEnumValueWithDescriptions_ThenDefaultReturned()
        {
            nameof(DummyEnum.StateA).GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateA);
            nameof(DummyEnum.StateB).GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateA);
            nameof(DummyEnum.StateC).GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateA);
        }

        [Fact]
        public void EnumExtensions_GivenGetValueFromDescription_WhenSearchingNameOfEnumValueWithoutDescriptions_ThenNameOfEnumValueReturned()
        {
            nameof(DummyEnum.StateD).GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateD);
            nameof(DummyEnum.StateE).GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateE);
            nameof(DummyEnum.StateF).GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateF);
        }
    }
}
