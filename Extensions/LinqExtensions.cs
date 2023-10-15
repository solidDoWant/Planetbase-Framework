using System;
using System.Collections.Generic;
using System.Linq;
using PlanetbaseFramework.DataStructures;
using Random = UnityEngine.Random;

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

        /// <summary>
        /// Randomly sorts items pseudo-randomly, using a Xorshift 128 algorithm, which results in a sort
        /// order that is close to a uniform random distribution.
        /// Note that this is not exactly uniform distribution as it relies on a call to OrderBy, which
        /// uses a "stable sort". This can result in two values next to each other in the source enumerable
        /// to remain next to each other. This has roughly a 1 in 10e14 chance of happening for any given pair.
        /// There is a roughly a 50% chance of this occurring in every 50 trillion number pairs.
        /// </summary>
        public static IEnumerable<T1> OrderRandom<T1>(this IEnumerable<T1> t1Enumerable) =>
            t1Enumerable.OrderBy(_ => Random.value);

        /// <summary>
        /// Returns a random item from the given collection, or default if there are no items in the collection.
        /// </summary>
        public static T1 RandomOrDefault<T1>(this ICollection<T1> t1Enumerable)
        {
            if (t1Enumerable.Count <= 1)
                return t1Enumerable.FirstOrDefault();

            var randomItemIndex = Random.Range(0, t1Enumerable.Count - 1);
            using (var enumerator = t1Enumerable.GetEnumerator())
            {
                for (var i = 0; i < randomItemIndex; i++)
                    if (!enumerator.MoveNext())
                        throw new Exception(
                            $"Attempted to get random item at {randomItemIndex}, but passed the end of the collection at position {i} first");

                return enumerator.Current;
            }
        }

        /// <summary>
        /// Filters out null results.
        /// </summary>
        public static IEnumerable<T1> NonNull<T1>(this IEnumerable<T1> t1Enumerable) =>
            t1Enumerable.Where(item => item != null);
    }
}