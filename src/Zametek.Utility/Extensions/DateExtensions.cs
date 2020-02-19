using System;
using System.Globalization;

namespace Zametek.Utility
{
    public static class DateExtensions
    {
        public static string ToISO8601String(this DateTime input)
        {
            return input.ToString(@"o", CultureInfo.InvariantCulture);
        }

        public static string ToISO8601String(this DateTimeOffset input)
        {
            return input.ToString(@"o", CultureInfo.InvariantCulture);
        }

        public static string ToUtcDisplayString(this DateTimeOffset input)
        {
            return input.ToString(@"u", CultureInfo.InvariantCulture);
        }
    }
}
