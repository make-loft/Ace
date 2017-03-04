using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    static class Enumerable
    {
        public static IEnumerable<T> Cast<T>(this IDictionary dictionary)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (T item in dictionary)
            {
                yield return item;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}
