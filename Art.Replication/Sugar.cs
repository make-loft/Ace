using System.Collections;
using System.Collections.Generic;

namespace Art
{
    public static class Cast
    {
        public static T Of<T>(this object o) => (T) o;
        public static bool Is<T>(this object o) => o is T;
        public static T As<T>(this object o) where T : class => o as T;
    }
}

namespace System.Linq
{
    public static class EnumerableArt
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection) action(item);
        }

        public static IEnumerable<T> Cast<T>(this IDictionary dictionary)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (T item in dictionary) yield return item;
        }
    }
}
