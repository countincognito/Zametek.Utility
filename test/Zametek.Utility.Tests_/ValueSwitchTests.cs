using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Zametek.Utility.Tests
{
    [TestClass]
    public class ValueSwitchTests
    {
        private readonly string A = nameof(A);
        private readonly string B = nameof(B);

        [TestMethod]
        public void ValueSwitch_NullString_NoException()
        {
            string item = null;
            item.ValueSwitchOn();
        }

        [TestMethod]
        public void ValueSwitch_NullString_DefaultExecuted()
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
            Assert.IsTrue(valueMatch);
            Assert.IsTrue(defaultExecuted);
        }

        [TestMethod]
        public void ValueSwitch_StringA_ValueMatchedDefaultNotExecuted()
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
            Assert.IsTrue(valueMatch);
            Assert.IsTrue(equalsA);
            Assert.IsFalse(defaultExecuted);
        }

        [TestMethod]
        public void ValueSwitch_StringA_ValueMatchedOnlyOnce()
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
            Assert.IsTrue(valueMatch);
            Assert.IsTrue(equalsA);
            Assert.IsFalse(alsoEqualsA);
            Assert.IsFalse(defaultExecuted);
        }

        [TestMethod]
        public void ValueSwitch_StringB_ValueMatchedAfterNonMatch()
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
            Assert.IsTrue(valueMatch);
            Assert.IsFalse(equalsA);
            Assert.IsTrue(equalsB);
            Assert.IsFalse(defaultExecuted);
        }
    }
}