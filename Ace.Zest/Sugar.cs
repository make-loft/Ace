using System;
using System.Collections.Generic;
using System.Linq;

namespace Ace
{
    public static class Sugar
    {
        public static bool IsNull(this object o) => o == null;

        public static bool IsNotNull(this object o) => o != null;

        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection) => collection == null || !collection.Any();

        public static TException Try<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
                return null;
            }
            catch (TException exception)
            {
                return exception;
            }
        }

        public static Exception Catch<TException>(this Exception exception, Action<Exception> action)
            where TException : Exception
        {
            if (exception.As<TException>() != null) action(exception);
            return exception;
        }

        public static Exception Try(Action action) => Try<Exception>(action);

        public static Exception Catch(this Exception exception, Action<Exception> action) => 
            exception.Catch<Exception>(action);

        public static void Finally(this Exception exception, Action action) => action();

        public static TResult With<TSource, TResult>(this TSource source, Func<TSource, TResult> action)
            where TSource : class => source == default(TSource) ? default(TResult) : action(source);

        public static TSource Make<TSource>(this TSource source, Action<TSource> action)
            where TSource : class
        {
            action(source);
            return source;
        }

        public static TSource Make<TSource>(this TSource source, Func<TSource, bool> condition, Action<TSource> action)
            where TSource : class
        {
            if (condition(source)) action(source);
            return source;
        }

        public static IEnumerable<T> Turn<T>(this IList<T> items, int skip, int turnsCount = 0)
        {
            var reverse = skip < 0;
            var count = items.Count;
            skip = reverse ? count + skip : skip;
            var take = turnsCount == 0
                ? reverse ? -skip - 1 : count - skip
                : count*turnsCount;
            return items.Ring(skip, take);
        }

        public static IEnumerable<T> Ring<T>(this IList<T> items, int skip, int take)
        {
            var reverse = take < 0;
            var count = items.Count;
            skip = skip < 0 ? count + skip : skip;
            skip = skip < count ? skip : skip%count;
            take = reverse ? -take : take;

            for (var i = 0; i < take; i++)
            {
                var j = i < count ? i : i%count;
                var index = reverse ? skip - j : skip + j;
                index = index < 0 ? count + index : index;
                index = index < count ? index : index%count;
                yield return items[index];
            }
        }

        // ReSharper disable PossibleMultipleEnumeration
        // ReSharper disable LoopCanBePartlyConvertedToQuery
        public static IEnumerable<T> SkipByRing<T>(this IEnumerable<T> source, int count)
        {
            var originalCount = 0;
            var reverse = count < 0;
            count = reverse ? -count : count;
            source = reverse ? source.Reverse() : source;

            while (true)
            {
                if (originalCount > 0) count %= originalCount;
                foreach (var item in source)
                {
                    originalCount++;
                    if (count > 0)
                    {
                        count--;
                        continue;
                    }
                    yield return item;
                }

                if (count == 0) yield break;
            }
        }

        public static IEnumerable<T> TakeByRing<T>(this IEnumerable<T> source, int count)
        {
            var reverse = count < 0;
            count = reverse ? -count : count;
            source = reverse ? source.Reverse() : source;

            while (true)
            {
                foreach (var item in source)
                {
                    if (count <= 0) continue;
                    count--;
                    yield return item;
                }

                if (count == 0) yield break;
            }
        }

        public static IEnumerable<T> SliceByRing<T>(this IEnumerable<T> source, int skipCount, int takeCount)
        {
            var originalCount = 0;
            var skipReverse = skipCount < 0;           
            var takeReverse = takeCount < 0;
            skipCount = skipReverse ? -skipCount : skipCount;
            takeCount = takeReverse ? -takeCount : takeCount;
            source = takeReverse ? source.Reverse() : source;

            if (skipReverse ^ takeReverse)
            {
                var count = source.Count();
                skipCount = count - skipCount % count;
            }

            while (true)
            {
                if (originalCount > 0) skipCount %= originalCount;
                foreach (var item in source)
                {
                    originalCount++;
                    if (skipCount > 0)
                    {
                        skipCount--;
                        continue;
                    }

                    if (takeCount > 0)
                    {
                        takeCount--;
                        yield return item;
                    }
                }

                if (takeCount == 0) yield break;
            }
        }
    }
}