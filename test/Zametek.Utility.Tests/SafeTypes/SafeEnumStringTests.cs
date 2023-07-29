using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Zametek.Utility.Tests
{
    public class SafeEnumStringTests
    {
        #region == and !=

        [Fact]
        public void SafeEnumString_GivenNullValue_WhenEquatedToNull_ThenIsEqual()
        {
            DummySafeEnumString test = null;
            (test is null).Should().BeTrue();
            (test == DummySafeEnumString.StateA).Should().BeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenNullValue_WhenEquatedToNotNull_ThenIsNotEqual()
        {
            DummySafeEnumString test = null;
            (test != null).Should().BeFalse();
            (test != DummySafeEnumString.StateA).Should().BeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenSameStringValue_WhenEquatedToSelf_ThenIsTrue()
        {
            string test = DummySafeEnumString.StateA.Value;
            (test == DummySafeEnumString.StateA).Should().BeTrue();
            (test != DummySafeEnumString.StateA).Should().BeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenOtherStringValue_WhenEquatedToSelf_ThenIsFalse()
        {
            string test = DummySafeEnumString.StateB.Value;
            (test == DummySafeEnumString.StateA).Should().BeFalse();
            (test != DummySafeEnumString.StateA).Should().BeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenSameSafeEnumValue_WhenEquatedToSelf_ThenIsTrue()
        {
            DummySafeEnumString test = DummySafeEnumString.StateA;
            (test == DummySafeEnumString.StateA).Should().BeTrue();
            (test != DummySafeEnumString.StateA).Should().BeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenEquatedToSelf_ThenIsFalse()
        {
            DummySafeEnumString test = DummySafeEnumString.StateB;
            (test == DummySafeEnumString.StateA).Should().BeFalse();
            (test != DummySafeEnumString.StateA).Should().BeTrue();
        }

        #endregion

        #region < and >=

        [Fact]
        public void SafeEnumString_GivenNullValue_WhenLessThanNull_ThenIsFalse()
        {
            DummySafeEnumString test = null;
            (test < null).Should().BeFalse();
            (test >= DummySafeEnumString.StateA).Should().BeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenSameSafeEnumValue_WhenLessThanSelf_ThenIsFalse()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test < DummySafeEnumString.StateC).Should().BeFalse();
            (test >= DummySafeEnumString.StateC).Should().BeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenLessThanLowerValue_ThenIsFalse()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test < DummySafeEnumString.StateA).Should().BeFalse();
            (test >= DummySafeEnumString.StateA).Should().BeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenLessThanHigherValue_ThenIsTrue()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test < DummySafeEnumString.StateF).Should().BeTrue();
            (test >= DummySafeEnumString.StateF).Should().BeFalse();
        }

        #endregion

        #region > and <=

        [Fact]
        public void SafeEnumString_GivenNullValue_WhenGreaterThanNull_ThenIsFalse()
        {
            DummySafeEnumString test = null;
            (test > null).Should().BeFalse();
            (test <= DummySafeEnumString.StateA).Should().BeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenSameSafeEnumValue_WhenGreaterThanSelf_ThenIsFalse()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test > DummySafeEnumString.StateC).Should().BeFalse();
            (test <= DummySafeEnumString.StateC).Should().BeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenGreaterThanLowerValue_ThenIsTrue()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test > DummySafeEnumString.StateA).Should().BeTrue();
            (test <= DummySafeEnumString.StateA).Should().BeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenGreaterThanHigherValue_ThenIsFalse()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test > DummySafeEnumString.StateF).Should().BeFalse();
            (test <= DummySafeEnumString.StateF).Should().BeTrue();
        }

        #endregion

        [Fact]
        public void SafeEnumString_GivenSameSafeEnumValue_WhenCallEqualsToSelf_ThenIsEqual()
        {
            DummySafeEnumString test = DummySafeEnumString.StateA;
            test.Equals(DummySafeEnumString.StateA).Should().BeTrue();
            (!test.Equals(DummySafeEnumString.StateA)).Should().BeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenCallEqualsToSelf_ThenIsNotEqual()
        {
            DummySafeEnumString test = DummySafeEnumString.StateB;
            test.Equals(DummySafeEnumString.StateA).Should().BeFalse();
            (!test.Equals(DummySafeEnumString.StateA)).Should().BeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenCheckOrder_WhenShuffledAndSorted_ThenShouldInCorrectOrder()
        {
            var test = new List<DummySafeEnumString>
            {
                DummySafeEnumString.StateA,
                DummySafeEnumString.StateB,
                DummySafeEnumString.StateC,
                DummySafeEnumString.StateD,
                DummySafeEnumString.StateE,
                DummySafeEnumString.StateF,
            };
            test.Shuffle();
            test.Should().NotBeInAscendingOrder();
            test.Sort();
            test.Should().BeInAscendingOrder();
        }

        [Fact]
        public void SafeEnumString_GivenCompareEquivalence_WhenShuffled_ThenShouldBeTrue()
        {
            var test = new List<DummySafeEnumString>
            {
                DummySafeEnumString.StateA,
                DummySafeEnumString.StateB,
                DummySafeEnumString.StateC,
                DummySafeEnumString.StateD,
                DummySafeEnumString.StateE,
                DummySafeEnumString.StateF,
            };
            var expected = new List<DummySafeEnumString>
            {
                DummySafeEnumString.StateA,
                DummySafeEnumString.StateB,
                DummySafeEnumString.StateC,
                DummySafeEnumString.StateD,
                DummySafeEnumString.StateE,
                DummySafeEnumString.StateF,
            };
            test.Shuffle();
            expected.Shuffle();
            test.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void SafeEnumString_GivenHashSetOfEnums_WhenCheckContainsEnums_ThenShouldBeTrue()
        {
            var test = new HashSet<DummySafeEnumString>
            {
                DummySafeEnumString.StateA,
                DummySafeEnumString.StateB,
                DummySafeEnumString.StateC,
            };
            test.Contains(DummySafeEnumString.StateA).Should().BeTrue();
            test.Contains(DummySafeEnumString.StateB).Should().BeTrue();
            test.Contains(DummySafeEnumString.StateC).Should().BeTrue();
            test.Contains(DummySafeEnumString.StateD).Should().BeFalse();
            test.Contains(DummySafeEnumString.StateE).Should().BeFalse();
            test.Contains(DummySafeEnumString.StateF).Should().BeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenGetAll_WhenCheckContainsEnums_ThenShouldBeTrue()
        {
            var all = SafeEnumStringHelper.GetAll<DummySafeEnumString>();
            all.Contains(DummySafeEnumString.StateA).Should().BeTrue();
            all.Contains(DummySafeEnumString.StateB).Should().BeTrue();
            all.Contains(DummySafeEnumString.StateC).Should().BeTrue();
            all.Contains(DummySafeEnumString.StateD).Should().BeTrue();
            all.Contains(DummySafeEnumString.StateE).Should().BeTrue();
            all.Contains(DummySafeEnumString.StateF).Should().BeTrue();
            all.Contains(new DummySafeEnumString("StateG")).Should().BeFalse();
        }
    }
}
