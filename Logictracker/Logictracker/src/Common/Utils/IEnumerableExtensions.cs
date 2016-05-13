using System;
using System.Collections.Generic;
using System.Linq;

namespace Logictracker.Utils
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<List<T>> InSetsOf<T>(this IEnumerable<T> source, int max)
        {
            var toReturn = new List<T>(max);
            foreach (var item in source)
            {
                toReturn.Add(item);
                if (toReturn.Count != max) continue;
                yield return toReturn;
                toReturn = new List<T>(max);
            }
            if (toReturn.Any())
            {
                yield return toReturn;
            }
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }
    }
}
