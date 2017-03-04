using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    static class Adapters
    {
        public static IEnumerable<T> Cast<T>(this IDictionary dictionary)
        {
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
