using System;

namespace Zametek.Utility
{
    public static class GuidExtensions
    {
        public static string ToDashedString(this Guid input)
        {
            return input.ToString(@"D");
        }

        public static string ToFlatString(this Guid input)
        {
            return input.ToString(@"N");
        }

        public static string ToPathString(this Guid input)
        {
            return input.ToFlatString();
        }
    }
}
