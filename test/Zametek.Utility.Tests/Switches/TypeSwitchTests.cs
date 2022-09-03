using FluentAssertions;
using Xunit;

namespace Zametek.Utility.Tests
{
    public class TypeSwitchTests
    {
        private class A
        {
        }
        private class B1 : A
        {
        }
        private class B2 : A
        {
        }

        [Fact]
        public void TypeSwitch_GivenNullObject_ThenNoException()
        {
            object item = null;
            item.TypeSwitchOn();
        }

        [Fact]
        public void TypeSwitch_GivenNullObject_ThenDefaultExecuted()
        {
            object item = null;
            bool referenceMatch = false;
            bool defaultExecuted = false;
            item.TypeSwitchOn()
                .Default(x =>
                {
                    referenceMatch = item == x;
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            defaultExecuted.Should().BeTrue();
        }

        [Fact]
        public void TypeSwitch_GivenObjectA_ThenGenericTypeMatchedDefaultNotExecuted()
        {
            object item = new A();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            item.TypeSwitchOn()
                .Case(typeof(A), x =>
                {
                    referenceMatch = item == x;
                    isOfTypeA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeTrue();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectA_ThenTypeMatchedDefaultNotExecuted()
        {
            object item = new A();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            item.TypeSwitchOn()
                .Case<A>(x =>
                {
                    referenceMatch = item == x;
                    isOfTypeA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeTrue();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectA_ThenGenericTypeMatchedOnlyOnce()
        {
            object item = new A();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            bool isAlsoOfTypeA = false;
            item.TypeSwitchOn()
                .Case<A>(x =>
                {
                    referenceMatch = item == x;
                    isOfTypeA = x is A;
                })
                .Case<A>(x =>
                {
                    isAlsoOfTypeA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeTrue();
            isAlsoOfTypeA.Should().BeFalse();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectA_ThenTypeMatchedOnlyOnce()
        {
            object item = new A();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            bool isAlsoOfTypeA = false;
            item.TypeSwitchOn()
                .Case(typeof(A), x =>
                {
                    referenceMatch = item == x;
                    isOfTypeA = x is A;
                })
                .Case(typeof(A), x =>
                {
                    isAlsoOfTypeA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeTrue();
            isAlsoOfTypeA.Should().BeFalse();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectB1_ThenParentGenericTypeMatchedDefaultNotExecuted()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            item.TypeSwitchOn()
                .Case<A>(x =>
                {
                    referenceMatch = item == x;
                    isOfTypeA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeTrue();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectB1_ThenParentTypeMatchedDefaultNotExecuted()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            item.TypeSwitchOn()
                .Case(typeof(A), x =>
                {
                    referenceMatch = item == x;
                    isOfTypeA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeTrue();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectB1_ThenParentGenericTypeMatchedBeforeActualType()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            bool isOfTypeB1 = false;
            item.TypeSwitchOn()
                .Case<A>(x =>
                {
                    referenceMatch = item == x;
                    isOfTypeA = x is A;
                })
                .Case<B1>(x =>
                {
                    isOfTypeB1 = x is B1;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeTrue();
            isOfTypeB1.Should().BeFalse();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectB1_ThenParentTypeMatchedBeforeActualType()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            bool isOfTypeB1 = false;
            item.TypeSwitchOn()
                .Case(typeof(A), x =>
                {
                    referenceMatch = item == x;
                    isOfTypeA = x is A;
                })
                .Case(typeof(B1), x =>
                {
                    isOfTypeB1 = x is B1;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeTrue();
            isOfTypeB1.Should().BeFalse();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectB1_ThenActualGenericTypeMatchedBeforeParentType()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            bool isOfTypeB1 = false;
            item.TypeSwitchOn()
                .Case<B1>(x =>
                {
                    referenceMatch = item == x;
                    isOfTypeB1 = x is B1;
                })
                .Case<A>(x =>
                {
                    isOfTypeA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeFalse();
            isOfTypeB1.Should().BeTrue();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectB1_ThenActualTypeMatchedBeforeParentType()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            bool isOfTypeB1 = false;
            item.TypeSwitchOn()
                .Case(typeof(B1), x =>
                {
                    referenceMatch = item == x;
                    isOfTypeB1 = x is B1;
                })
                .Case(typeof(A), x =>
                {
                    isOfTypeA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeFalse();
            isOfTypeB1.Should().BeTrue();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectB1_ThenActualGenericTypeMatchedAfterNonMatch()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            bool isOfTypeB1 = false;
            bool isOfTypeB2 = false;
            item.TypeSwitchOn()
                .Case<B2>(x =>
                {
                    isOfTypeB2 = x is B2;
                })
                .Case<B1>(x =>
                {
                    referenceMatch = item == x;
                    isOfTypeB1 = x is B1;
                })
                .Case<A>(x =>
                {
                    isOfTypeA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeFalse();
            isOfTypeB1.Should().BeTrue();
            isOfTypeB2.Should().BeFalse();
            defaultExecuted.Should().BeFalse();
        }

        [Fact]
        public void TypeSwitch_GivenObjectB1_ThenActualTypeMatchedAfterNonMatch()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isOfTypeA = false;
            bool isOfTypeB1 = false;
            bool isOfTypeB2 = false;
            item.TypeSwitchOn()
                .Case(typeof(B2), x =>
                {
                    isOfTypeB2 = x is B2;
                })
                .Case(typeof(B1), x =>
                {
                    referenceMatch = item == x;
                    isOfTypeB1 = x is B1;
                })
                .Case(typeof(A), x =>
                {
                    isOfTypeA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            referenceMatch.Should().BeTrue();
            isOfTypeA.Should().BeFalse();
            isOfTypeB1.Should().BeTrue();
            isOfTypeB2.Should().BeFalse();
            defaultExecuted.Should().BeFalse();
        }
    }
}