using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable InconsistentNaming
	public static partial class LE
	{
		public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, object> map, TKey key, TValue fallbackValue = default) =>
			map.TryGetValue(key, out var value) ? (TValue)value : fallbackValue;

		public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> map, TKey key, TValue fallbackValue = default) =>
			map.TryGetValue(key, out var value) ? value : fallbackValue;

		public static Switch<T> ToSwitch<T>(this T value, params object[] pattern) => new(value, pattern);

		public static KeyValuePair<TK, TV> Of<TK, TV>(this TK key, TV value) => new(key, value);
		public static KeyValuePair<TK, TV> By<TK, TV>(this TV value, TK key) => new(key, value);

		public static T Or<T>(this T o, T x) where T : class => o ?? x;
		public static T Or<T>(this T? o, T x) where T : struct => o ?? x;
		public static T OrNew<T>(this T o) where T : class, new() => o ?? new T();
		public static T OrNew<T>(this T o, ref T x) where T : class, new() => o ?? (x = new T());
		
		/* a hack to define the .NET Framework runtime todo with C# 7.3 */
		private static readonly bool IsNetFrameworkRuntime =
			typeof(Environment).GetProperties()[0].Name != "CommandLine";

		public static Uri ToUri(this string value) => new(value);
		/* use 'typeof(Uri).SetNonPublicStaticField("s_IriParsing", false); // s_IdnScope = 2||0' to allow 'skipEscape' */
		public static Uri ToUri(this string value, bool skipEscape = false) => new(value, skipEscape);
		public static Regex ToRegex(this string value, RegexOptions options = RegexOptions.Compiled) => new(value, options);
		public static Guid ToGuid(this string value) => new(value);

		public static bool EqualsAsStrings(this object a, object b,
			StringComparison comparison = StringComparison.CurrentCultureIgnoreCase) =>
			ReferenceEquals(a, b) || string.Compare(a.To<string>(), b.To<string>(), comparison).Is(0);

		public static string GetPath(this Environment.SpecialFolder folder) => Environment.GetFolderPath(folder);

		public static StringBuilder Append(this StringBuilder builder, params object[] args)
		{
			for (var i = 0; i < args.Length; i++) builder.Append(args[i]);
			return builder;
		}

#if NET45 || XAMARIN
		public static async Task<TResult> ToAsync<TResult>(this TResult result) => await Task.FromResult(result);
		public static TResult SyncAwait<TResult>(this Task<TResult> task) => task.GetAwaiter().GetResult();
		public static void SyncAwait(this Task task) => task.GetAwaiter().GetResult();
#endif

		public static T Setup<T>(this T o, Func<T, Setup<T>> block) => block(o).Setters;
	}

	public struct Setup<T>
	{
		public Setup(T value) => Setters = Items = value;
		public T Setters { get; set; }
		public T Items { get; set; }
	}
}