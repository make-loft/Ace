using System.Collections;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    public static class Reflection
    {
        public static T GetCustomAttribute<T>(this Type type) where T: class =>
            type.GetCustomAttributes(true).FirstOrDefault(a => a is T) as T;
    }

    internal static class Enumerable
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
