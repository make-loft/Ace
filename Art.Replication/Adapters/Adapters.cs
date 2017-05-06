﻿using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    internal static class Reflection
    {
        public static T GetCustomAttribute<T>(this Type type) where T: class =>
            type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;

        public static T GetCustomAttribute<T>(this MemberInfo member) where T : class =>
            member?.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
    }
}

namespace System
{
    public static class TypeInfo
    {
        public static Type GetTypeInfo(this Type type) => type;
    }
}
