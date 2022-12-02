using System;
using System.Text;
using System.Globalization;

using static System.Globalization.NumberStyles;
using static System.Globalization.NumberFormatInfo;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Ace
{
	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
		public static bool IsNotNullOrEmpty(this string value) => !string.IsNullOrEmpty(value);
		public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);
		public static bool IsNotNullOrWhiteSpace(this string value) => !string.IsNullOrWhiteSpace(value);
		public static string Format(this string value, params object[] args) => string.Format(value, args);
		public static string Format(this string value, IFormatProvider provider, params object[] args) =>
			string.Format(provider, value, args);

		private static readonly Dictionary<string, char[]> stringToChars = new();

		public static char[] GetCachedChars(this string value) => stringToChars.TryGetValue(value, out var chars)
			? chars
			: stringToChars[value] = value.ToCharArray();

		public static string[] SplitByChars(this string value, string separators, bool keepEmptyEntries = false) =>
			value?.Split(separators.GetCachedChars(), keepEmptyEntries ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);

		public static bool Match(this string original, string pattern, int offset)
		{
			if (offset + pattern.Length > original.Length) return false;

			for (var i = 0; i < pattern.Length && offset + i < original.Length; i++)
			{
				if (original[offset + i].IsNot(pattern[i])) return false;
			}

			return true;
		}

		public static StringBuilder Append(this StringBuilder builder, params string[] values)
		{
			foreach (var value in values) builder.Append(value);
			return builder;
		}

		public static char Pick(this string str, int index) => index < 0 ? str[str.Length + index] : str[index];
		public static char PickByRing(this string str, int index) => str.Pick(index % str.Length);

		public static bool TryParse(this string pattern, out bool value) => bool.TryParse(pattern, out value);
		public static bool TryParse(this string pattern, out byte value) => byte.TryParse(pattern, Integer, InvariantInfo, out value);
		public static bool TryParse(this string pattern, out sbyte value) => sbyte.TryParse(pattern, Integer, InvariantInfo, out value);
		public static bool TryParse(this string pattern, out char value) => char.TryParse(pattern, out value);

		public static bool TryParse(this string pattern, out int value) => int.TryParse(pattern, Integer, InvariantInfo, out value);
		public static bool TryParse(this string pattern, out uint value) => uint.TryParse(pattern, Integer, InvariantInfo, out value);
		public static bool TryParse(this string pattern, out long value) => long.TryParse(pattern, Integer, InvariantInfo, out value);
		public static bool TryParse(this string pattern, out ulong value) => ulong.TryParse(pattern, Integer, InvariantInfo, out value);
		public static bool TryParse(this string pattern, out short value) => short.TryParse(pattern, Integer, InvariantInfo, out value);
		public static bool TryParse(this string pattern, out ushort value) => ushort.TryParse(pattern, Integer, InvariantInfo, out value);

		public static bool TryParse(this string pattern, out float value) => float.TryParse(pattern, Any, InvariantInfo, out value);
		public static bool TryParse(this string pattern, out double value) => double.TryParse(pattern, Any, InvariantInfo, out value);
		public static bool TryParse(this string pattern, out decimal value) => decimal.TryParse(pattern, Any, InvariantInfo, out value);

		public static bool TryParse(this string pattern, out DateTime value) => DateTime.TryParse(pattern, out value);
		public static bool TryParse(this string pattern, IFormatProvider provider, out DateTime value) =>
			DateTime.TryParse(pattern, provider, default, out value);
		public static bool TryParse(this string pattern, IFormatProvider provider, DateTimeStyles styles, out DateTime value) =>
			DateTime.TryParse(pattern, provider, styles, out value);
	}
}