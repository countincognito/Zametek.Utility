using System;
using System.Globalization;

namespace Zametek.Utility
{
    public static class GuidExtensions
    {
        public static string ToDashedString(this Guid input)
        {
            return input.ToString(@"D", CultureInfo.InvariantCulture);
        }

        public static string ToFlatString(this Guid input)
        {
            return input.ToString(@"N", CultureInfo.InvariantCulture);
        }

        public static string ToPathString(this Guid input)
        {
            return input.ToFlatString();
        }
    }
}
