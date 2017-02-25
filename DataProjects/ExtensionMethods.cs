using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProjects
{
    public static class ExtensionMethods
    {
        public static decimal ToNearestHalf(this decimal d)
        {
            var multi = d * 2;
            var round = Math.Round(multi, MidpointRounding.AwayFromZero);
            return round / 2;
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int n)
        {
            return source.Skip(Math.Max(0, source.Count() - n));
        }

        public static double WeightedAverage(this IEnumerable<double> source, int n)
        {
            var aggregated = source.Take(n).ToList();
            aggregated.AddRange(source);
            return aggregated.Average(x => x);
        }

        public static double WeightedAverage(this IEnumerable<int> source, int n)
        {
            var aggregated = source.TakeLast(n).ToList();
            aggregated.AddRange(source);
            return aggregated.Average(x => x);
        }

        public static Dictionary<string, int> ToNamesAndPlaces(this IEnumerable<string> fileLines)
        {
            var result = fileLines
                .Select(tab => tab.Split('\t'))
                .Select(x => x[0])
                .ToList();

            var namesAndPlaces = result.ToDictionary(x => x, x => result.IndexOf(x));
            return namesAndPlaces;
        }

    }
}
