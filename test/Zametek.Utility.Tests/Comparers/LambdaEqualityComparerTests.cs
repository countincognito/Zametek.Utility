using AutoFixture;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Zametek.Utility.Tests
{
    public class LambdaEqualityComparerTests
    {
        [Fact]
        public void LambdaEqualityComparerTests_GivenRandomObject_ThenShouldAlwaysEqualToItself()
        {
            var comparer = new LambdaEqualityComparer<DummyObjectSerializable>((a, b) =>
            {
                return
                    a.IntegerValue.Equals(b.IntegerValue)
                    && a.DoubleValue.Equals(b.DoubleValue)
                    && a.StringValue.Equals(b.StringValue);
            });

            var fixture = new Fixture();

            for (int i = 0; i < 20; i++)
            {
                var item = fixture.Build<DummyObjectSerializable>()
                    .Create();

                var itemCopy = item.CloneObject();
                comparer.Equals(item, itemCopy).Should().BeTrue();
            }
        }

        [Fact]
        public void LambdaEqualityComparerTests_GivenRandomObjectPairs_ThenShouldAlwaysBeDifferent()
        {
            var comparer = new LambdaEqualityComparer<DummyObject>((a, b) =>
            {
                return
                    a.IntegerValue.Equals(b.IntegerValue)
                    && a.DoubleValue.Equals(b.DoubleValue)
                    && a.StringValue.Equals(b.StringValue);
            });

            var fixture = new Fixture();

            for (int i = 0; i < 20; i++)
            {
                var items = fixture.Build<DummyObject>()
                    .CreateMany(2)
                    .ToList();
                comparer.Equals(items[0], items[1]).Should().BeFalse();
            }
        }
    }
}
