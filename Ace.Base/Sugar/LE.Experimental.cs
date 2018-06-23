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

		public static Switch<T> ToSwitch<T>(this T value, params object[] pattern) => new Switch<T>(value, pattern);

		public static KeyValuePair<TK, TV> Of<TK, TV>(this TK key, TV value) => new KeyValuePair<TK, TV>(key, value);
		public static KeyValuePair<TK, TV> By<TK, TV>(this TV value, TK key) => new KeyValuePair<TK, TV>(key, value);

		public static bool EqualsAsStrings(this object a, object b,
			StringComparison comparison = StringComparison.CurrentCultureIgnoreCase) =>
			ReferenceEquals(a, b) || string.Compare(a.ToStr(), b.ToStr(), comparison).Is(0);
		
		public static T Or<T>(this T o, T x) where T : class => o ?? x;
		public static T Or<T>(this T? o, T x) where T : struct => o ?? x;
		public static T OrNew<T>(this T o) where T : class, new() => o ?? new T();
		
		/* a hack to define the .NET Framework runtime todo with C# 7.3 */
		private static readonly bool IsNetFrameworkRuntime =
			typeof(Environment).GetProperties()[0].Name != "CommandLine";
	}
}