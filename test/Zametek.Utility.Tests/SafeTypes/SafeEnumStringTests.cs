using Shouldly;
using System.Collections.Generic;
using System.Linq;
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
            (test is null).ShouldBeTrue();
            (test == DummySafeEnumString.StateA).ShouldBeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenNullValue_WhenEquatedToNotNull_ThenIsNotEqual()
        {
            DummySafeEnumString test = null;
            (test != null).ShouldBeFalse();
            (test != DummySafeEnumString.StateA).ShouldBeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenSameStringValue_WhenEquatedToSelf_ThenIsTrue()
        {
            string test = DummySafeEnumString.StateA.Value;
            (test == DummySafeEnumString.StateA).ShouldBeTrue();
            (test != DummySafeEnumString.StateA).ShouldBeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenOtherStringValue_WhenEquatedToSelf_ThenIsFalse()
        {
            string test = DummySafeEnumString.StateB.Value;
            (test == DummySafeEnumString.StateA).ShouldBeFalse();
            (test != DummySafeEnumString.StateA).ShouldBeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenSameSafeEnumValue_WhenEquatedToSelf_ThenIsTrue()
        {
            DummySafeEnumString test = DummySafeEnumString.StateA;
            (test == DummySafeEnumString.StateA).ShouldBeTrue();
            (test != DummySafeEnumString.StateA).ShouldBeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenEquatedToSelf_ThenIsFalse()
        {
            DummySafeEnumString test = DummySafeEnumString.StateB;
            (test == DummySafeEnumString.StateA).ShouldBeFalse();
            (test != DummySafeEnumString.StateA).ShouldBeTrue();
        }


        [Fact]
        public void SafeEnumString_GivenIdenticalStringValue_WhenCaseIsIdentical_ThenIsTrue()
        {
            string stateAString = @"StateA";
            DummySafeEnumString test = DummySafeEnumString.StateA;
            (test == stateAString).ShouldBeTrue();
            (test != stateAString).ShouldBeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenIdenticalStringValue_WhenCaseIsDifferent_ThenIsFalse()
        {
            string stateAString = @"sTaTeA";
            DummySafeEnumString test = DummySafeEnumString.StateA;
            (test == stateAString).ShouldBeFalse();
            (test != stateAString).ShouldBeTrue();
        }

        #endregion

        #region < and >=

        [Fact]
        public void SafeEnumString_GivenNullValue_WhenLessThanNull_ThenIsFalse()
        {
            DummySafeEnumString test = null;
            (test < null).ShouldBeFalse();
            (test >= DummySafeEnumString.StateA).ShouldBeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenSameSafeEnumValue_WhenLessThanSelf_ThenIsFalse()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test < DummySafeEnumString.StateC).ShouldBeFalse();
            (test >= DummySafeEnumString.StateC).ShouldBeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenLessThanLowerValue_ThenIsFalse()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test < DummySafeEnumString.StateA).ShouldBeFalse();
            (test >= DummySafeEnumString.StateA).ShouldBeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenLessThanHigherValue_ThenIsTrue()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test < DummySafeEnumString.StateF).ShouldBeTrue();
            (test >= DummySafeEnumString.StateF).ShouldBeFalse();
        }

        #endregion

        #region > and <=

        [Fact]
        public void SafeEnumString_GivenNullValue_WhenGreaterThanNull_ThenIsFalse()
        {
            DummySafeEnumString test = null;
            (test > null).ShouldBeFalse();
            (test <= DummySafeEnumString.StateA).ShouldBeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenSameSafeEnumValue_WhenGreaterThanSelf_ThenIsFalse()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test > DummySafeEnumString.StateC).ShouldBeFalse();
            (test <= DummySafeEnumString.StateC).ShouldBeTrue();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenGreaterThanLowerValue_ThenIsTrue()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test > DummySafeEnumString.StateA).ShouldBeTrue();
            (test <= DummySafeEnumString.StateA).ShouldBeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenGreaterThanHigherValue_ThenIsFalse()
        {
            DummySafeEnumString test = DummySafeEnumString.StateC;
            (test > DummySafeEnumString.StateF).ShouldBeFalse();
            (test <= DummySafeEnumString.StateF).ShouldBeTrue();
        }

        #endregion

        [Fact]
        public void SafeEnumString_GivenSameSafeEnumValue_WhenCallEqualsToSelf_ThenIsEqual()
        {
            DummySafeEnumString test = DummySafeEnumString.StateA;
            test.Equals(DummySafeEnumString.StateA).ShouldBeTrue();
            (!test.Equals(DummySafeEnumString.StateA)).ShouldBeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenOtherSafeEnumValue_WhenCallEqualsToSelf_ThenIsNotEqual()
        {
            DummySafeEnumString test = DummySafeEnumString.StateB;
            test.Equals(DummySafeEnumString.StateA).ShouldBeFalse();
            (!test.Equals(DummySafeEnumString.StateA)).ShouldBeTrue();
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
            test.SequenceEqual(test.OrderBy(x => x)).ShouldBeFalse();
            test.Sort();
            test.ShouldBeInOrder(SortDirection.Ascending);
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
            test.ShouldBe(expected, ignoreOrder: true);
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
            test.Contains(DummySafeEnumString.StateA).ShouldBeTrue();
            test.Contains(DummySafeEnumString.StateB).ShouldBeTrue();
            test.Contains(DummySafeEnumString.StateC).ShouldBeTrue();
            test.Contains(DummySafeEnumString.StateD).ShouldBeFalse();
            test.Contains(DummySafeEnumString.StateE).ShouldBeFalse();
            test.Contains(DummySafeEnumString.StateF).ShouldBeFalse();
        }

        [Fact]
        public void SafeEnumString_GivenGetAll_WhenCheckContainsEnums_ThenShouldBeTrue()
        {
            var all = SafeEnumStringHelper.GetAll<DummySafeEnumString>();
            all.Contains(DummySafeEnumString.StateA).ShouldBeTrue();
            all.Contains(DummySafeEnumString.StateB).ShouldBeTrue();
            all.Contains(DummySafeEnumString.StateC).ShouldBeTrue();
            all.Contains(DummySafeEnumString.StateD).ShouldBeTrue();
            all.Contains(DummySafeEnumString.StateE).ShouldBeTrue();
            all.Contains(DummySafeEnumString.StateF).ShouldBeTrue();
            all.Contains(new DummySafeEnumString("StateG")).ShouldBeFalse();
        }
    }
}
