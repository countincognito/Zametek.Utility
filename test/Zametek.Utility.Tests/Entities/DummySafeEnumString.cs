namespace Zametek.Utility.Tests
{
    public class DummySafeEnumString
        : SafeEnumString<DummySafeEnumString>
    {
        public static readonly DummySafeEnumString StateA = new DummySafeEnumString(nameof(StateA));
        public static readonly DummySafeEnumString StateB = new DummySafeEnumString(nameof(StateB));
        public static readonly DummySafeEnumString StateC = new DummySafeEnumString(nameof(StateC));
        public static readonly DummySafeEnumString StateD = new DummySafeEnumString(nameof(StateD));
        public static readonly DummySafeEnumString StateE = new DummySafeEnumString(nameof(StateE));
        public static readonly DummySafeEnumString StateF = new DummySafeEnumString(nameof(StateF));

        public DummySafeEnumString(string value)
            : base(value)
        {
        }
    }
}
