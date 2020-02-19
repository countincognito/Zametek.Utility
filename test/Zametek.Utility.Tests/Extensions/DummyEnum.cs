using System.ComponentModel;

namespace Zametek.Utility.Tests
{
    public enum DummyEnum
    {
        [Description(@"A")]
        StateA,
        [Description(@"B")]
        StateB,
        [Description(@"C")]
        StateC,

        StateD,
        StateE,
        StateF,
    }
}
