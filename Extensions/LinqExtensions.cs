using System;
using System.Collections.Generic;
using System.Linq;
using PlanetbaseFramework.DataStructures;

// ReSharper disable UnusedMember.Global

namespace PlanetbaseFramework.Extensions
{
    public static class LinqExtensions
    {
        public static IEnumerable<Tuple<T1, T2>> Zip<T1, T2>(this IEnumerable<T1> t1Enumerable,
            IEnumerable<T2> t2Enumerable)
        {
            using (var t1Enumerator = t1Enumerable.GetEnumerator())
            using (var t2Enumerator = t2Enumerable.GetEnumerator())
            {
                while (true)
                {
                    var t1MoveSucceeded = t1Enumerator.MoveNext();
                    var t2MoveSucceeded = t2Enumerator.MoveNext();

                    if (!t1MoveSucceeded && !t2MoveSucceeded)
                        yield break;

                    var t1Value = t1MoveSucceeded ? t1Enumerator.Current : default;
                    var t2Value = t2MoveSucceeded ? t2Enumerator.Current : default;

                    yield return new Tuple<T1, T2>(t1Value, t2Value);
                }
            }
        }

        public static bool AreCollectionsIdentical<T1>(this IEnumerable<T1> firstEnumerable,
            IEnumerable<T1> secondEnumerable, Func<T1, T1, bool> comparisonFunction)
        {
            return firstEnumerable.Zip(secondEnumerable).All(pair => comparisonFunction(pair.Item1, pair.Item2));
        }
    }
}