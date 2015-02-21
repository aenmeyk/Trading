using System;
using System.Collections.Generic;

namespace Common.ExtensionMethods
{
    public static class EnumerableExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            var random = new Random();
            var i = list.Count;

            while (i > 1)
            {
                i--;
                var k = random.Next(i + 1);
                T value = list[k];
                list[k] = list[i];
                list[i] = value;
            }
        }
    }
}
