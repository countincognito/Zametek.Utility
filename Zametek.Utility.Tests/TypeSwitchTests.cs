using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zametek.Utility.UnitTests
{
    [TestClass]
    public class TypeSwitchTests
    {
        private class A
        { }
        private class B1 : A
        { }
        private class B2 : A
        { }

        [TestMethod]
        public void TypeSwitch_NullObject_NoException()
        {
            object item = null;
            item.TypeSwitchOn();
        }

        [TestMethod]
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

        [TestMethod]
        public void TypeSwitch_ObjectA_TypeMatchedDefaultNotExecuted()
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
            Assert.IsTrue(referenceMatch);
            Assert.IsTrue(isOfTypeA);
            Assert.IsFalse(defaultExecuted);
        }

        [TestMethod]
        public void TypeSwitch_ObjectA_TypeMatchedOnlyOnce()
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
            Assert.IsTrue(referenceMatch);
            Assert.IsTrue(isOfTypeA);
            Assert.IsFalse(isAlsoOfTypeA);
            Assert.IsFalse(defaultExecuted);
        }

        [TestMethod]
        public void TypeSwitch_ObjectB1_ParentTypeMatchedDefaultNotExecuted()
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
            Assert.IsTrue(referenceMatch);
            Assert.IsTrue(isOfTypeA);
            Assert.IsFalse(defaultExecuted);
        }

        [TestMethod]
        public void TypeSwitch_ObjectB1_ParentTypeMatchedBeforeActualType()
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
            Assert.IsTrue(referenceMatch);
            Assert.IsTrue(isOfTypeA);
            Assert.IsFalse(isOfTypeB1);
            Assert.IsFalse(defaultExecuted);
        }

        [TestMethod]
        public void TypeSwitch_ObjectB1_ActualTypeMatchedBeforeParentType()
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
            Assert.IsTrue(referenceMatch);
            Assert.IsFalse(isOfTypeA);
            Assert.IsTrue(isOfTypeB1);
            Assert.IsFalse(defaultExecuted);
        }

        [TestMethod]
        public void TypeSwitch_ObjectB1_ActualTypeMatchedAfterNonMatch()
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
            Assert.IsTrue(referenceMatch);
            Assert.IsFalse(isOfTypeA);
            Assert.IsTrue(isOfTypeB1);
            Assert.IsFalse(isOfTypeB2);
            Assert.IsFalse(defaultExecuted);
        }
    }
}