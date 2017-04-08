using System.Linq;

// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    internal static class Reflection
    {
        public static T GetCustomAttribute<T>(this Type type) where T: class =>
            type.GetCustomAttributes(true).FirstOrDefault(a => a is T) as T;

        public static T GetCustomAttribute<T>(this MemberInfo member) where T : class =>
            member?.GetCustomAttributes(true).FirstOrDefault(a => a is T) as T;
    }
}
