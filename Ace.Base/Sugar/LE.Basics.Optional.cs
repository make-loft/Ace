using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace, InconsistentNaming
namespace Ace
{
	/* The Language Extensions */
	public static partial class LE_Optional
	{
		/* boxing free value matching */
		public static bool Equals<T>(T a, in T b) => EqualityComparer<T>.Default.Equals(a, b);
		public static bool Equals<T>(in T a, in T b) => EqualityComparer<T>.Default.Equals(a, b);
		public static bool Equals<T>(in T a, T b) => EqualityComparer<T>.Default.Equals(a, b);

		/* boxing free value matching */
		public static bool Is<T>(this T o, in T x) => Equals(in o, in x); /* Equals<T>(in o, in x); */
		public static bool Is<T>(this T o, in object x) => x.Is<T>() && Equals(in o, (T)x);
		public static bool Is<T>(this object o, in T x) => o.Is<T>() && Equals((T)o, in x);
		public static bool Is(this object o, in object x) => Equals(in o, in x);

		public static bool IsNot<T>(this T o, in T x) => !o.Is(x);
		public static bool IsNot<T>(this object o, in T x) => !o.Is(x);
		public static bool IsNot<T>(this T o, in object x) => !o.Is(x);
		public static bool IsNot(this object o, in object x) => !o.Is(x);

		public static bool Is<T>(this T? o, in T x) where T : struct => o.HasValue && Equals(o.Value, in x);
		public static bool Is<T>(this T o, in T? x) where T : struct => x.HasValue && Equals(in o, x.Value);

		public static bool IsNot<T>(this T? o, in T x) where T : struct => !o.Is(in x);
		public static bool IsNot<T>(this T o, in T? x) where T : struct => !o.Is(in x);

		/* string based value matching */
		public static int Compare(this string o, in string x, in StringComparison comparison = StringComparison.Ordinal) =>
			string.Compare(o, x, comparison);

		public static bool Is(this string o, in string x, in StringComparison comparison) => o.Compare(x, comparison).Is(0);
		public static bool Is(this object o, in object x, in StringComparison comparison) => o.Is(x) || o.ToStr().Is(x.ToStr(), comparison);
		public static bool IsNot(this string o, in string x, in StringComparison comparison) => !o.Is(x, comparison);
		public static bool IsNot(this object o, in object x, in StringComparison comparison) => !o.Is(x, comparison);

		public static Uri ToUri(this string uriString) => new Uri(uriString);
		public static Uri ToUri(this string uriString, out Uri uri) => uri = new Uri(uriString);
		public static Regex ToRegex(this string pattern, RegexOptions options = RegexOptions.Compiled) => new Regex(pattern, options);
		public static Guid ToGuid(this string uriString) => new Guid(uriString);

#if NET45 || XAMARIN
		public static async Task<TResult> ToAsync<TResult>(this TResult result) => await Task.FromResult(result);
#endif
	}
}