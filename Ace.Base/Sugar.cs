using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Ace
{
	public static class LanguageExtensions
	{
		public static bool Not(this bool b) => !b;
		
		public static void Let<T>(this T o) { }
		public static TR Let<T, TR>(this T o, TR y) => y;
		public static TR Let<T, TR>(this T o, TR y, out T x) => (x = o).Let(y);

		public static T To<T>(this T o) => o;
		public static T To<T>(this object o) => (T) Convert.ChangeType(o, typeof(T), null);
		public static T To<T>(this T o, out T x) => x = o;
		public static T To<T>(this object o, out T x) => x = (T) Convert.ChangeType(o, typeof(T), null);

		public static T To<T, TR>(this T o, Func<T, TR> decomposer, out TR x) =>
			(x = o == null ? default(TR) : decomposer(o)).Let(o);

		public static T As<T>(this T o) where T : class => o;
		public static T As<T>(this object o) where T : class => o as T;
		public static T As<T>(this T o, out T x) where T : class => x = o;
		public static T As<T>(this object o, out T x) where T : class => x = o as T;

		public static bool IsNull<T>(this T o) => o == null;
		public static bool IsNull<T>(this T o, out T x) => (x = o) == null;

		public static bool Is<T>(this T o) => o != null; /* or same 'o is T' */
		public static bool Is<T>(this object o) => o is T;
		public static bool Is<T>(this T o, out T x) => (x = o) != null; /* or same '(x = o) is T' */
		public static bool Is<T>(this object o, out T x) => (x = o is T ? (T) o : default(T)) != null;
		public static bool Is<T>(this T o, T x) => Equals(o, x);
		public static bool Is<T>(this object o, T x) => Equals(o, x);
		public static bool Is<T>(this T? o, T x) where T : struct => Equals(o, x);

		public static bool Is<T>(this T o, Func<T, bool> checker) => o != null && checker(o);
		public static bool Is<T>(this object o, Func<T, bool> checker) => o is T && checker((T) o);

		public static bool Is<T>(this T o, Func<T, bool> checker, out T x) =>
			o != null ? checker(x = o) : (x = default(T)) != null;

		public static bool Is<T>(this object o, Func<T, bool> checker, out T x) =>
			o is T ? checker(x = (T) o) : (x = default(T)) != null;

		public static Switch<T> Match<T>(this T value, params object[] pattern) =>
			new Switch<T>(value, pattern);
	}

	public class Switch<T>
	{
		private readonly T _value;
		private object[] _pattern;

		public Switch(T value) => _value = value;
		public Switch(T value, object[] pattern) : this(value) => _pattern = pattern;

		public bool Case(params object[] pattern)
		{
			pattern = pattern ?? new object[] {null};
			_pattern = _pattern ?? new object[] {_value};
			for (var i = 0; i < pattern.Length && i < _pattern.Length; i++)
			{
				if (Equals(pattern[i], _pattern[i])) continue;
				return false;
			}

			return true;
		}

		public bool Case<TValue>() => _value is TValue;
		public bool Case(out T value) => Case<T>(out value);

		public bool Case<TValue>(out TValue value) where TValue : T =>
			(value = (_value is TValue).To(out var b) ? (TValue) _value : default(TValue)).Let(b);
	}

	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);
		public static bool IsNotNullOrEmpty(this string value) => !string.IsNullOrEmpty(value);
		public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);
		public static bool IsNotNullOrWhiteSpace(this string value) => !string.IsNullOrWhiteSpace(value);
		public static string Format(this string value, string format, params object[] args) => string.Format(format, args);
		public static string Format(this string value, IFormatProvider provider, string format, params object[] args) =>
			string.Format(provider, format, args);

		public static bool Match(this string original, string pattern, int offset)
		{
			if (offset + pattern.Length > original.Length) return false;

			for (var i = 0; i < pattern.Length && offset + i < original.Length; i++)
			{
				if (original[offset + i] != pattern[i]) return false;
			}

			return true;
		}

		public static StringBuilder Append(this StringBuilder builder, params string[] values)
		{
			foreach (var value in values) builder.Append(value);
			return builder;
		}
	}
}

namespace System.Linq
{
	public static class EnumerableExtensions
	{
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection) action(item);
		}

		public static IEnumerable<T> Cast<T>(this IDictionary dictionary)
		{
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (T item in dictionary) yield return item;
		}

		public static IEnumerable<T> ToEnumerable<T>(this T singleItem)
		{
			yield return singleItem;
		}

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, T singleItem) =>
			collection.Concat(singleItem.ToEnumerable());

		public static Dictionary<TKey,TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items) =>
			items.ToDictionary(p => p.Key, p => p.Value);

		public static IList<T> AppendRange<T>(this IList<T> target, IEnumerable<T> source)
		{
			foreach (var item in source) target.Add(item);
			return target;
		}

		public static IDictionary<TKey, TValue> AppendRange<TKey, TValue>(this IDictionary<TKey, TValue> target,
			IEnumerable<KeyValuePair<TKey, TValue>> source)
		{
			foreach (var item in source) target.Add(item);
			return target;
		}

		internal static void CopyToMultidimensionalArray(this IList<object> source, Array target, IList<int> dimensions)
		{
			var indices = new int[dimensions.Count];
			for (var i = 0; i < source.Count; i++)
			{
				var t = i;
				for (var j = indices.Length - 1; j >= 0; j--)
				{
					indices[j] = t % dimensions[j];
					t /= dimensions[j];
				}

				target.SetValue(source[i], indices);
			}
		}

		internal static int[] RestoreDimensions(this IList items, int rank)
		{
			var dimensions = new int[rank];

			for (var i = 0; i < rank; i++)
			{
				items = items[0] is IList l ? l : items;
				dimensions[i] = items.Count;
			}

			return dimensions;
		}

		internal static T BoxMultidimensionArray<T>(this IEnumerable items, IList<int> dimensions,
			Func<IEnumerable<object>, T> box)
		{
			var chunks = items.Cast<object>();
			for (var i = dimensions.Count - 1; i >= 0; i--)
			{
				var dimension = dimensions[i];
				chunks = chunks.Chunk(dimension).Select(box).Cast<object>();
			}

			return box(chunks);
		}

		internal static IEnumerable<object> UnboxMultidimensionArray(this IEnumerable items, int rank)
		{
			foreach (var item in items)
			{
				if (item is IEnumerable s && rank > 0)
				{
					var subitems = UnboxMultidimensionArray(s, rank - 1);
					foreach (var subitem in subitems)
						yield return subitem;
				}
				else yield return item;
			}
		}

		public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
		{
			using (var enumerator = source.GetEnumerator())
			{
				do
				{
					if (!enumerator.MoveNext()) yield break;
					yield return ChunkSequence(enumerator, chunkSize);
				} while (true);
			}
		}

		private static IEnumerable<T> ChunkSequence<T>(IEnumerator<T> enumerator, int chunkSize)
		{
			var count = 0;

			do
			{
				yield return enumerator.Current;
			} while (++count < chunkSize && enumerator.MoveNext());
		}
	}
}