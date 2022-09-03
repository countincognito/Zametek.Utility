namespace Zametek.Utility.Tests
{
    public class DummySafeEnumString
        : SafeEnumString<DummySafeEnumString>
    {
        public static readonly DummySafeEnumString StateA = new(nameof(StateA));
        public static readonly DummySafeEnumString StateB = new(nameof(StateB));
        public static readonly DummySafeEnumString StateC = new(nameof(StateC));
        public static readonly DummySafeEnumString StateD = new(nameof(StateD));
        public static readonly DummySafeEnumString StateE = new(nameof(StateE));
        public static readonly DummySafeEnumString StateF = new(nameof(StateF));

        public DummySafeEnumString(string value)
            : base(value)
        {
        }
    }
}
