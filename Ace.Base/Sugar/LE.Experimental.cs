using System;
using System.Collections.Generic;
// ReSharper disable once CheckNamespace
namespace Ace
{
    // ReSharper disable InconsistentNaming
    public static partial class LE
    {
        public static bool Not(this bool b) => !b;
        public static bool IsTrue(this bool b) => b;
        public static bool IsFalse(this bool b) => !b;
        public static string ToStr(this string o) => o;
        public static string ToStr(this object o) => o?.ToString();

        public static Switch<T> Match<T>(this T value, params object[] pattern) => new Switch<T>(value, pattern);

        public static KeyValuePair<TK, TV> To<TK, TV>(this TK key, TV value) => new KeyValuePair<TK, TV>(key, value);
        public static KeyValuePair<TK, TV> By<TK, TV>(this TV value, TK key) => new KeyValuePair<TK, TV>(key, value);

        public static bool EqualsAsStrings(this object a, object b,
            StringComparison comparison = StringComparison.CurrentCultureIgnoreCase) =>
            ReferenceEquals(a, b) || string.Compare(a.ToStr(), b.ToStr(), comparison) == 0;
    }
}