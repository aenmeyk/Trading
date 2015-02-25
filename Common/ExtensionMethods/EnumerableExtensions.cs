using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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

        public static double StandardDeviation(this IEnumerable<double> values)
        {
            var standardDeviation = 0.0;
            var count = values.Count();

            if (count > 1)
            {
                //Compute the Average
                var average = values.Average();

                //Perform the Sum of (value-avg)^2
                var sum = values.Sum(d => (d - average) * (d - average));

                //Put it all together
                standardDeviation = Math.Sqrt(sum / count);
            }

            return standardDeviation;
        }

        public static double RelativeStandardDeviation(this IEnumerable<double> values)
        {
            return values.StandardDeviation() / values.Average();
        }
    }
}
