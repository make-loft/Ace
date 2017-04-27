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

    public static class SrtringExtensions
    {
        public static bool Match(this string original, string substring, int offset)
        {
            if (offset + substring.Length > original.Length) return false;

            for (var i = 0; i < substring.Length && offset + i < original.Length; i++)
            {
                if (original[offset + i] != substring[i]) return false;
            }

            return true;
        }
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
