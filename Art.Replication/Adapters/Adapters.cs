﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    internal static class Reflection
    {
        public static T GetCustomAttribute<T>(this Type type) where T: class =>
            type.GetCustomAttributes(true).FirstOrDefault(a => a is T) as T;

        public static T GetCustomAttribute<T>(this MemberInfo member) where T : class =>
            member?.GetCustomAttributes(true).FirstOrDefault(a => a is T) as T;
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
