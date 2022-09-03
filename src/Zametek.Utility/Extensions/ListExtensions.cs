using System;
using System.Collections.Generic;

namespace Zametek.Utility
{
    public static class ListExtensions
    {
        private static readonly Random s_Rand = new Random();

        public static IEnumerable<List<T>> ToBatches<T>(this List<T> items, int batchSize)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (batchSize < 1)
            {
                throw new ArgumentException(Properties.Resources.CannotHaveValueLessThanOne, nameof(batchSize));
            }

            for (int i = 0; i < items.Count; i += batchSize)
            {
                yield return items.GetRange(i, Math.Min(batchSize, items.Count - i));
            }
        }

        public static List<T> ToBatch<T>(this List<T> items, int batchNumber, int batchSize)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            if (batchNumber < 1)
            {
                throw new ArgumentException(Properties.Resources.CannotHaveValueLessThanOne, nameof(batchNumber));
            }
            if (batchSize < 1)
            {
                throw new ArgumentException(Properties.Resources.CannotHaveValueLessThanOne, nameof(batchSize));
            }

            int startIndex = (batchNumber - 1) * batchSize;

            if (startIndex < items.Count)
            {
                return items.GetRange(startIndex, Math.Min(batchSize, items.Count - startIndex));
            }

            return new List<T>();
        }

        public static T RandomItem<T>(this List<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            return items[s_Rand.Next(items.Count)];
        }

        public static void Shuffle<T>(this IList<T> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            int count = items.Count;
            while (count > 1)
            {
                count--;
                int newIndex = s_Rand.Next(count + 1);
                (items[count], items[newIndex]) = (items[newIndex], items[count]);
            }
        }
    }
}
