using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Ace.Comparers;

namespace Ace
{
    public static class Cloning
    {
        public static List<Type> LikeImmutableTypes = new List<Type> {typeof(string), typeof(Regex)};

        public static T MemberwiseClone<T>(this T origin, bool deepMode,
            IEqualityComparer<object> comparer = null) => deepMode
            ? (T) origin.GetDeepClone(new Dictionary<object, object>(comparer ?? ReferenceComparer<object>.Default))
            : (T) MemberwiseCloneMethod.Invoke(origin, null);

        private static readonly MethodInfo MemberwiseCloneMethod =
            typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        private static IEnumerable<FieldInfo> EnumerateFields(this Type type, BindingFlags bindingFlags) =>
            type.BaseType?.EnumerateFields(bindingFlags)
                .Concat(type.GetFields(bindingFlags | BindingFlags.DeclaredOnly)) ??
            type.GetFields(bindingFlags);

        private static bool IsLikeImmutable(this Type type) => type.IsValueType || LikeImmutableTypes.Contains(type);

        private static object GetDeepClone(this object origin, IDictionary<object, object> originToClone)
        {
            if (origin == null) return null;
            var type = origin.GetType();
            if (type.IsLikeImmutable()) return origin;

            if (originToClone.TryGetValue(origin, out var clone)) return clone;
            clone = MemberwiseCloneMethod.Invoke(origin, null);
            originToClone.Add(origin, clone);

            if (type.IsArray && !type.GetElementType().IsLikeImmutable())
            {
                var array = (Array) clone;
                var indices = new int[array.Rank];
                var dimensions = new int[array.Rank];
                for (var i = 0; i < array.Rank; i++) dimensions[i] = array.GetLength(i);
                for (var i = 0; i < array.Length; i++)
                {
                    var t = i;
                    for (var j = indices.Length - 1; j >= 0; j--)
                    {
                        indices[j] = t % dimensions[j];
                        t /= dimensions[j];
                    }

                    var deepClone = array.GetValue(indices).GetDeepClone(originToClone);
                    array.SetValue(deepClone, indices);
                }
            }

            var fields = type.EnumerateFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields.Where(f => !f.FieldType.IsLikeImmutable()))
            {
                var deepClone = field.GetValue(origin).GetDeepClone(originToClone);
                field.SetValue(origin, deepClone);
            }

            return clone;
        }
    }
}
