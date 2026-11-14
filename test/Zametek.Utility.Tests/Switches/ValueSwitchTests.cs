using Shouldly;
using Xunit;

namespace Zametek.Utility.Tests
{
    public class ValueSwitchTests
    {
        private readonly string A = nameof(A);
        private readonly string B = nameof(B);

        [Fact]
        public void ValueSwitch_GivenNullString_ThenNoException()
        {
            string item = null;
            item.ValueSwitchOn();
        }

        [Fact]
        public void ValueSwitch_GivenNullString_ThenDefaultExecuted()
        {
            string item = null;
            bool valueMatch = false;
            bool defaultExecuted = false;
            item.ValueSwitchOn()
                .Default(x =>
                {
                    valueMatch = string.Equals(item, x);
                    defaultExecuted = true;
                });
            valueMatch.ShouldBeTrue();
            defaultExecuted.ShouldBeTrue();
        }

        [Fact]
        public void ValueSwitch_GivenStringA_ThenValueMatchedDefaultNotExecuted()
        {
            string item = A;
            bool valueMatch = false;
            bool defaultExecuted = false;
            bool equalsA = false;
            item.ValueSwitchOn()
                .Case(A, x =>
                {
                    valueMatch = string.Equals(item, x);
                    equalsA = string.Equals(x, A);
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            valueMatch.ShouldBeTrue();
            equalsA.ShouldBeTrue();
            defaultExecuted.ShouldBeFalse();
        }

        [Fact]
        public void ValueSwitch_GivenStringA_ThenValueMatchedOnlyOnce()
        {
            string item = A;
            bool valueMatch = false;
            bool defaultExecuted = false;
            bool equalsA = false;
            bool alsoEqualsA = false;
            item.ValueSwitchOn()
                .Case(A, x =>
                {
                    valueMatch = string.Equals(item, x);
                    equalsA = string.Equals(x, A);
                })
                .Case(A, x =>
                {
                    alsoEqualsA = string.Equals(x, A);
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            valueMatch.ShouldBeTrue();
            equalsA.ShouldBeTrue();
            alsoEqualsA.ShouldBeFalse();
            defaultExecuted.ShouldBeFalse();
        }

        [Fact]
        public void ValueSwitch_GivenStringB_ThenValueMatchedAfterNonMatch()
        {
            string item = B;
            bool valueMatch = false;
            bool defaultExecuted = false;
            bool equalsA = false;
            bool equalsB = false;
            item.ValueSwitchOn()
                .Case(A, x =>
                {
                    equalsA = string.Equals(x, A);
                })
                .Case(B, x =>
                {
                    valueMatch = string.Equals(item, x);
                    equalsB = string.Equals(x, B);
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            valueMatch.ShouldBeTrue();
            equalsA.ShouldBeFalse();
            equalsB.ShouldBeTrue();
            defaultExecuted.ShouldBeFalse();
        }
    }
}