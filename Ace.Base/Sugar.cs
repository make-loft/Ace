using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ace
{
	/* LanguageExtensions */
	// ReSharper disable once InconsistentNaming
	public static class LE
	{
		public static object ChangeType(this object o, Type type) =>
			o == null || type.IsValueType || o is IConvertible ? Convert.ChangeType(o, type, null) : o;

		public static void Let<T>(this T o) { }
		public static TR Let<T, TR>(this T o, TR y) => y;
		public static TR Let<T, TR>(this T o, ref TR y) => y;
		public static TR Let<T, TR>(this T o, TR y, out T x) => (x = o).Let(y);

		public static T Dec<T>(out T x, T value = default(T)) => x = value;
		public static TL Dec<TL, T>(this TL o, out T x, T value = default(T)) => (x = value).Let(o);

		public static T To<T>(this T o) => o;
		public static T To<T>(this object o) => (T) ChangeType(o, typeof(T));
		public static T To<T>(this T o, out T x) => x = o;
		public static T To<T>(this object o, out T x) => x = (T) ChangeType(o, typeof(T));

		public static string ToStr(this object o) => o?.ToString();
		public static string ToStr(this string o) => o;

		public static T As<T>(this T o) where T : class => o;
		public static T As<T>(this object o) where T : class => o as T;
		public static T As<T>(this T o, out T x) where T : class => x = o;
		public static T As<T>(this object o, out T x) where T : class => x = o as T;

		public static bool IsNull<T>(this T o) => !typeof(T).IsValueType && o == null; // is null
		public static bool IsNull<T>(this T? o) where T: struct => !o.HasValue; // is null
		public static bool IsNull<T>(this T o, out T x) => (x = o).IsNull();

		public static bool Is<T>(this T o) => typeof(T).IsValueType || o != null; // is T
		public static bool Is<T>(this T? o) where T: struct => o.HasValue; // is T
		public static bool Is<T>(this object o) => o is T;
	
		public static bool Is<T>(this object o, out T x, T fallbackValue = default(T)) =>
			(x = o is T ? (T) o : fallbackValue).Is(); // is T

		public static bool Is<T>(this T o, T x) => EqualityComparer<T>.Default.Equals(o, x);
		public static bool Is<T>(this T o, object x) => x is T && EqualityComparer<T>.Default.Equals(o, (T) x);
		public static bool Is<T>(this object o, T x) => o is T && EqualityComparer<T>.Default.Equals((T) o, x);

		public static bool Is<T>(this T? o, T x) where T : struct =>
			o.HasValue && EqualityComparer<T>.Default.Equals(o.Value, x);

		public static bool Is<T>(this T o, T? x) where T : struct =>
			x.HasValue && EqualityComparer<T>.Default.Equals(o, x.Value);
		
		public static bool IsNot<T>(this T o, T x) => !o.Is(x);
		public static bool IsNot<T>(this object o, T x) => !o.Is(x);
		public static bool IsNot<T>(this T o, object x) => !o.Is(x);
		public static bool IsNot<T>(this T? o, T x) where T : struct => !o.Is(x);
		public static bool Not(this bool b) => !b;

		public static T With<T>(this T o, params object[] pattern) => o;
		public static T With<T, T0>(this T o, T0 a) => o;
		public static T With<T, T0, T1>(this T o, T0 a, T1 b) => o;
		public static T With<T, T0, T1, T2>(this T o, T0 a, T1 b, T2 c) => o;
		public static T With<T, T0, T1, T2, T3>(this T o, T0 a, T1 b, T2 c, T3 d) => o;
		public static T With<T, T0, T1, T2, T3, T4>(this T o, T0 a, T1 b, T2 c, T3 d, T4 e) => o;
		public static T With<T, T0, T1, T2, T3, T4, T5>(this T o, T0 a, T1 b, T2 c, T3 d, T4 e, T5 f) => o;
		public static T With<T, T0, T1, T2, T3, T4, T5, T6>(this T o, T0 a, T1 b, T2 c, T3 d, T4 e, T5 f, T6 g) => o;
		public static T With<T, T0, T1, T2, T3, T4, T5, T6, T7>
			(this T o, T0 a, T1 b, T2 c, T3 d, T4 e, T5 f, T6 g, T7 h) => o;

		public static TCollection Merge<TCollection, T>(this TCollection collection, params T[] items)
			where TCollection : ICollection<T>
		{
			items.ForEach(collection.Add); // foreach (var item in items) collection.Add(item);
			return collection;
		}

		public static bool AndAll(this bool o, params bool[] pattern) => o && pattern.All(b => b);
		public static bool AndAny(this bool o, params bool[] pattern) => o && pattern.Any(b => b);
		public static bool OrAll(this bool o, params bool[] pattern) => o || pattern.All(b => b);
		public static bool OrAny(this bool o, params bool[] pattern) => o || pattern.Any(b => b);
		public static bool XorAll(this bool o, params bool[] pattern) => o ^ pattern.All(b => b);
		public static bool XorAny(this bool o, params bool[] pattern) => o ^ pattern.Any(b => b);

		public static KeyValuePair<TK, TV> To<TK, TV>(this TK key, TV value) => new KeyValuePair<TK, TV>(key, value);
		public static KeyValuePair<TK, TV> By<TK, TV>(this TV value, TK key) => new KeyValuePair<TK, TV>(key, value);

		public static Switch<T> Match<T>(this T value, params object[] pattern) => new Switch<T>(value, pattern);
	}

	public class Switch<T>
	{
		private readonly object _value;
		private object[] _pattern;

		public Switch(T value) => _value = value;
		public Switch(T value, object[] pattern) : this(value) => _pattern = pattern;

		public bool Case(params object[] pattern)
		{
			pattern = pattern ?? new[] {(object) null};
			_pattern = _pattern ?? new[] {_value};
			for (var i = 0; i < pattern.Length && i < _pattern.Length; i++)
			{
				if (Equals(pattern[i], _pattern[i])) continue;
				return false;
			}

			return true;
		}

		public bool Case<TValue>() where TValue : T => _value.Is<TValue>();
		public bool Case(out T value) => Case<T>(out value);

		public bool Case<TValue>(out TValue value, TValue fallbackValue = default(TValue)) where TValue : T =>
			_value.Is(out value, fallbackValue);
	}

	public static class New
	{	
		public static T Object<T>() where T : new() => new T();

		public static T Object<T>(params object[] constructorArgs) =>
			(T) Activator.CreateInstance(typeof(T), constructorArgs);

		public static object Object(Type type, params object[] constructorArgs) =>
			Activator.CreateInstance(type, constructorArgs);
		
		public static T[] Array<T>(params T[] items) => items;
		
		public static List<T> List<T>(params T[] items) => new List<T>(items);

		public static Dictionary<TK, TV> Dictionary<TK, TV>(params KeyValuePair<TK, TV>[] items) =>
			new Dictionary<TK, TV>().Merge(items);
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

		public static bool EqualsAsStrings(this object a, object b,
			StringComparison comparison = StringComparison.CurrentCultureIgnoreCase) =>
			a == b
			|| string.Compare(a as string, b?.ToString(), comparison) == 0
			|| string.Compare(a?.ToString(), b as string, comparison) == 0;
		
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
			foreach (T item in dictionary) yield return item;
		}

		public static IEnumerable<T> ToEnumerable<T>(this T singleItem)
		{
			yield return singleItem;
		}

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, T singleItem) =>
			collection.Concat(singleItem.ToEnumerable());

		public static IEnumerable<T> Concat<T>(this IEnumerable<T> collection, params T[] items) =>
			Enumerable.Concat(collection, items);

		public static Dictionary<TKey, TValue>
			ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items) =>
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