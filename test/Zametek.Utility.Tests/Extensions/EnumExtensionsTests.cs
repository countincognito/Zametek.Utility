using FluentAssertions;
using System;
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

        [Theory]
        [InlineData(DummyEnum.StateA, @"A")]
        [InlineData(DummyEnum.StateB, @"B")]
        [InlineData(DummyEnum.StateC, @"C")]
        [InlineData(DummyEnum.StateD, nameof(DummyEnum.StateD))]
        [InlineData(DummyEnum.StateE, nameof(DummyEnum.StateE))]
        [InlineData(DummyEnum.StateF, nameof(DummyEnum.StateF))]
        public void EnumExtensions_GivenGetDescription_WhenEnumHasDescription_ThenDescriptionReturned(
            DummyEnum @enum,
            string description)
        {
            @enum.GetDescription().Should().Be(description);
        }

        [Theory]
        [InlineData(@"A", DummyEnum.StateA)]
        [InlineData(@"B", DummyEnum.StateB)]
        [InlineData(@"C", DummyEnum.StateC)]
        public void EnumExtensions_GivenGetValueFromDescription_WhenEnumHasDescription_ThenDescriptionReturned(
            string description,
            DummyEnum @enum)
        {
            description.GetValueFromDescription<DummyEnum>().Should().Be(@enum);
        }

        [Fact]
        public void EnumExtensions_GivenGetValueFromDescription_WhenSearchingRandomDescription_ThenDefaultReturned()
        {
            for (int i = 0; i < 10; i++)
            {
                Guid.NewGuid().ToFlatString().GetValueFromDescription<DummyEnum>().Should().Be(DummyEnum.StateA);
            }
        }

        [Theory]
        [InlineData(nameof(DummyEnum.StateA), DummyEnum.StateA)]
        [InlineData(nameof(DummyEnum.StateB), DummyEnum.StateA)]
        [InlineData(nameof(DummyEnum.StateC), DummyEnum.StateA)]
        [InlineData(nameof(DummyEnum.StateD), DummyEnum.StateD)]
        [InlineData(nameof(DummyEnum.StateE), DummyEnum.StateE)]
        [InlineData(nameof(DummyEnum.StateF), DummyEnum.StateF)]
        public void EnumExtensions_GivenGetValueFromDescription_WhenSearchingNameOfEnumValueWithDescriptions_ThenDefaultReturned(
            string description,
            DummyEnum @enum)
        {
            description.GetValueFromDescription<DummyEnum>().Should().Be(@enum);
        }
    }
}
