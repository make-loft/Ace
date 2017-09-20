using System;
using System.Linq;

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

namespace System.TypeInfoAdapter
{
    public static class TypeInfo
    {
        public static Type GetTypeInfo(this Type type) => type;
    }
}

namespace Ace.Adapters
{
    [Flags]
    public enum BindingFlags
    {
        DeclaredOnly = 2,
        ExactBinding = 65536,
        FlattenHierarchy = 64,
        IgnoreCase = 1,
        Instance = 4,
        NonPublic = 32,
        OptionalParamBinding = 262144,
        Public = 16,
        Static = 8,
    }
}
