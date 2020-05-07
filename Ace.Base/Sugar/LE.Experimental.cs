﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable InconsistentNaming
	public static partial class LE
	{
		public static Switch<T> ToSwitch<T>(this T value, params object[] pattern) => new Switch<T>(value, pattern);

		public static KeyValuePair<TK, TV> Of<TK, TV>(this TK key, TV value) => new KeyValuePair<TK, TV>(key, value);
		public static KeyValuePair<TK, TV> By<TK, TV>(this TV value, TK key) => new KeyValuePair<TK, TV>(key, value);

		public static T Or<T>(this T o, T x) where T : class => o ?? x;
		public static T Or<T>(this T? o, T x) where T : struct => o ?? x;
		public static T OrNew<T>(this T o) where T : class, new() => o ?? new T();
		public static T OrNew<T>(this T o, ref T x) where T : class, new() => o ?? (x = new T());
		
		/* a hack to define the .NET Framework runtime todo with C# 7.3 */
		private static readonly bool IsNetFrameworkRuntime =
			typeof(Environment).GetProperties()[0].Name != "CommandLine";

		public static Uri ToUri(this string value, bool skipEscape = false) => new Uri(value, skipEscape);
		public static Regex ToRegex(this string value, RegexOptions options = RegexOptions.Compiled) => new Regex(value, options);
		public static Guid ToGuid(this string value) => new Guid(value);

		public static string ToStr(this string o) => o;
		public static string ToStr(this object o) => o?.ToString();

		public static bool EqualsAsStrings(this object a, object b,
			StringComparison comparison = StringComparison.CurrentCultureIgnoreCase) =>
			ReferenceEquals(a, b) || string.Compare(a.ToStr(), b.ToStr(), comparison).Is(0);

		public static string GetPath(this Environment.SpecialFolder folder) => Environment.GetFolderPath(folder);

#if NET45 || XAMARIN
		public static async Task<TResult> ToAsync<TResult>(this TResult result) => await Task.FromResult(result);
		public static TResult SyncAwait<TResult>(this Task<TResult> task) => task.GetAwaiter().GetResult();
		public static void SyncAwait(this Task task) => task.GetAwaiter().GetResult();
#endif
	}
}