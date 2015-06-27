using NUnit.Framework;

namespace Zametek.Utility.UnitTests
{
    [TestFixture]
    public class TypeSwitchTests
    {
        private class A
        { }
        private class B1 : A
        { }
        private class B2 : A
        { }

        [Test]
        public void TypeSwitch_NullObject_NoException()
        {
            object item = null;
            item.TypeSwitchOn();
            Assert.Pass();
        }

        [Test]
        public void TypeSwitch_NullObject_DefaultExecuted()
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
            Assert.IsTrue(referenceMatch);
            Assert.IsTrue(defaultExecuted);
        }

        [Test]
        public void TypeSwitch_NullObject_TypeMatchedDefaultNotExecuted()
        {
            object item = new A();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isTypeOfA = false;
            item.TypeSwitchOn()
                .Case<A>(x =>
                {
                    referenceMatch = item == x;
                    isTypeOfA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            Assert.IsTrue(referenceMatch);
            Assert.IsTrue(isTypeOfA);
            Assert.IsFalse(defaultExecuted);
        }

        [Test]
        public void TypeSwitch_NullObject_SubTypeMatchedDefaultNotExecuted()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isTypeOfA = false;
            item.TypeSwitchOn()
                .Case<A>(x =>
                {
                    referenceMatch = item == x;
                    isTypeOfA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            Assert.IsTrue(referenceMatch);
            Assert.IsTrue(isTypeOfA);
            Assert.IsFalse(defaultExecuted);
        }

        [Test]
        public void TypeSwitch_NullObject_SubTypeMatchedBeforeActualType()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isTypeOfA = false;
            bool isTypeOfB1 = false;
            item.TypeSwitchOn()
                .Case<A>(x =>
                {
                    referenceMatch = item == x;
                    isTypeOfA = x is A;
                })
                .Case<B1>(x =>
                {
                    isTypeOfB1 = x is B1;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            Assert.IsTrue(referenceMatch);
            Assert.IsTrue(isTypeOfA);
            Assert.IsFalse(isTypeOfB1);
            Assert.IsFalse(defaultExecuted);
        }

        [Test]
        public void TypeSwitch_NullObject_ActualTypeMatchedBeforeSubType()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isTypeOfA = false;
            bool isTypeOfB1 = false;
            item.TypeSwitchOn()
                .Case<B1>(x =>
                {
                    referenceMatch = item == x;
                    isTypeOfB1 = x is B1;
                })
                .Case<A>(x =>
                {
                    isTypeOfA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            Assert.IsTrue(referenceMatch);
            Assert.IsFalse(isTypeOfA);
            Assert.IsTrue(isTypeOfB1);
            Assert.IsFalse(defaultExecuted);
        }

        [Test]
        public void TypeSwitch_NullObject_ActualTypeMatchedAfterNonMatch()
        {
            object item = new B1();
            bool referenceMatch = false;
            bool defaultExecuted = false;
            bool isTypeOfA = false;
            bool isTypeOfB1 = false;
            bool isTypeOfB2 = false;
            item.TypeSwitchOn()
                .Case<B2>(x =>
                {
                    isTypeOfB2 = x is B2;
                })
                .Case<B1>(x =>
                {
                    referenceMatch = item == x;
                    isTypeOfB1 = x is B1;
                })
                .Case<A>(x =>
                {
                    isTypeOfA = x is A;
                })
                .Default(x =>
                {
                    defaultExecuted = true;
                });
            Assert.IsTrue(referenceMatch);
            Assert.IsFalse(isTypeOfA);
            Assert.IsTrue(isTypeOfB1);
            Assert.IsFalse(isTypeOfB2);
            Assert.IsFalse(defaultExecuted);
        }
    }
}