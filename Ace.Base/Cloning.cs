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
        public static readonly List<Type> LikeImmutableTypes = new List<Type> {typeof(Regex)};

        private static readonly MethodInfo MemberwiseCloneMethod =
            typeof(object).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);

        public static T MemberwiseClone<T>(this T origin, bool deepMode,
            IEqualityComparer<object> comparer = null, Type[] likeImmutableTypes = null) => deepMode
            ? (T) origin.GetDeepClone(new Dictionary<object, object>(comparer ?? ReferenceComparer<object>.Default),
                likeImmutableTypes ?? LikeImmutableTypes.ToArray())
            : (T) MemberwiseCloneMethod.Invoke(origin, null);

        private static IEnumerable<FieldInfo> EnumerateFields(this Type type, BindingFlags bindingFlags) =>
            type.BaseType?.EnumerateFields(bindingFlags)
                .Concat(type.GetFields(bindingFlags | BindingFlags.DeclaredOnly)) ??
            type.GetFields(bindingFlags);

        private static bool IsLikeImmutable(this Type type, Type[] likeImmutableTypes) =>
            type.IsValueType || type == typeof(string) || likeImmutableTypes.Contains(type);

        private static object GetDeepClone(this object origin,
            IDictionary<object, object> originToClone, Type[] likeImmutableTypes) =>
            origin == null ? null :
            origin.GetType().Is(out var type) && type.IsLikeImmutable(likeImmutableTypes) ? origin :
            originToClone.TryGetValue(origin, out var deepClone) ? deepClone :
            (originToClone[origin] = MemberwiseCloneMethod.Invoke(origin, null))
            .MakeDeep(type, o => o.GetDeepClone(originToClone, likeImmutableTypes));

        private static object MakeDeep(this object origin, Type type, Func<object, object> getDeepClone) =>
            origin.CloneByFields(type, getDeepClone) is Array array ? array.CloneByIndices(getDeepClone) : origin;
        
        private static object CloneByFields(this object origin, Type type, Func<object, object> getDeepClone)
        {
            var fields = type.EnumerateFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields)
            {
                var deepClone = getDeepClone(field.GetValue(origin));
                field.SetValue(origin, deepClone);
            }

            return origin;
        }

        private static Array CloneByIndices(this Array array, Func<object, object> getDeepClone)
        {
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

                var deepClone = getDeepClone(array.GetValue(indices));
                array.SetValue(deepClone, indices);
            }
            
            return array;
        }
    }
}
