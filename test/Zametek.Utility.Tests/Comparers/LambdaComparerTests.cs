using AutoFixture;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Zametek.Utility.Tests
{
    public class LambdaComparerTests
    {
        [Fact]
        public void LambdaComparerTests_GivenRandomObject_ThenShouldAlwaysEqualToItself()
        {
            var comparer = new LambdaComparer<DummyObjectSerializable>((a, b) =>
            {
                return
                    a.IntegerValue.CompareTo(b.IntegerValue)
                    * a.DoubleValue.CompareTo(b.DoubleValue)
                    * a.StringValue.CompareTo(b.StringValue);
            });

            var fixture = new Fixture();

            for (int i = 0; i < 20; i++)
            {
                var item = fixture.Build<DummyObjectSerializable>()
                    .Create();

                var itemCopy = item.CloneObject();
                comparer.Compare(item, itemCopy).Should().Be(0);
            }
        }

        [Fact]
        public void LambdaComparerTests_GivenRandomObjectPairs_ThenShouldAlwaysBeDifferent()
        {
            var comparer = new LambdaComparer<DummyObject>((a,b) =>
            {
                return
                    a.IntegerValue.CompareTo(b.IntegerValue)
                    * a.DoubleValue.CompareTo(b.DoubleValue)
                    * a.StringValue.CompareTo(b.StringValue);
            });

            var fixture = new Fixture();

            for (int i = 0; i < 20; i++)
            {
                var items = fixture.Build<DummyObject>()
                    .CreateMany(2)
                    .ToList();
                comparer.Compare(items[0], items[1]).Should().NotBe(0);
            }
        }
    }
}
