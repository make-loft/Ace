using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ace;

namespace Ace
{
	/* LanguageExtensions */
	// ReSharper disable once InconsistentNaming
	public static class LE
	{
		public static object ChangeType(this object o, Type type) =>
			o == null || type.IsValueType || o is IConvertible ? Convert.ChangeType(o, type, null) : o;

		public static void Like<T>(this T o) { }
		public static TR Like<T, TR>(this T o, TR y) => y;
		public static TR Like<T, TR>(this T o, ref TR y) => y;
		public static TR Like<T, TR>(this T o, TR y, out T x) => (x = o).Like(y);

		public static T Dec<T>(out T x, T value = default(T)) => x = value;
		public static TL Dec<TL, T>(this TL o, out T x, T value = default(T)) => (x = value).Like(o);
		
		public static string ToStr(this object o) => o?.ToString();
		public static string ToStr(this string o) => o;

		public static T To<T>(this T o) => o;
		public static T To<T>(this T o, out T x) => x = o;
		public static T To<T>(this object o) => (T) ChangeType(o, typeof(T));
		public static T To<T>(this object o, out T x) => x = (T) ChangeType(o, typeof(T));

		public static T As<T>(this T o) => o;
		public static T As<T>(this T o, out T x) => x = o;
		public static T As<T>(this object o, T fallbackValue = default(T)) => o is T ? (T) o : fallbackValue;
		public static T As<T>(this object o, out T x, T fallbackValue = default(T)) => x = o.As(fallbackValue);

		public static bool Is<T>(this T o) => typeof(T).IsValueType || o != null; // o is T
		public static bool Is<T>(this T? o) where T: struct => o.HasValue; // o is T
		public static bool Is<T>(this object o) => o is T; // o != null && typeof(T).IsAssignableFrom(o.GetType());
		
		public static bool IsNull<T>(this T o) => !typeof(T).IsValueType && o == null; // is null
		public static bool IsNull<T>(this T? o) where T: struct => !o.HasValue; // is null
		public static bool IsNull<T>(this T o, out T x) => (x = o).IsNull();

		public static bool Is<T>(this T o, out T x) => o.To(out x).Is(); // is T	
		public static bool Is<T>(this object o, out T x, T fallbackValue = default(T)) =>
			(x = o.Is<T>().To(out var b) ? (T) o : fallbackValue).Like(b); // is T

		public static bool Is<T>(this T o, T x) => EqualityComparer<T>.Default.Equals(o, x);
		public static bool Is<T>(this T o, object x) => x.Is<T>() && EqualityComparer<T>.Default.Equals(o, (T) x);
		public static bool Is<T>(this object o, T x) => o.Is<T>() && EqualityComparer<T>.Default.Equals((T) o, x);

		public static bool Is<T>(this T? o, T x) where T : struct =>
			o.HasValue && EqualityComparer<T>.Default.Equals(o.Value, x);

		public static bool Is<T>(this T o, T? x) where T : struct =>
			x.HasValue && EqualityComparer<T>.Default.Equals(o, x.Value);
		
		public static bool IsNot<T>(this T o, T x) => !o.Is(x);
		public static bool IsNot<T>(this object o, T x) => !o.Is(x);
		public static bool IsNot<T>(this T o, object x) => !o.Is(x);
		public static bool IsNot<T>(this T? o, T x) where T : struct => !o.Is(x);
		public static bool Not(this bool b) => !b;

		public delegate void WithAction();
		public delegate void WithAction<in T>(T o);

		public static T With<T>(this T o, WithAction action)
		{
			action();
			return o;
		}
		
		public static T With<T>(this T o, WithAction<T> action)
		{
			action(o);
			return o;
		}

		// ReSharper disable InconsistentNaming
		
		public static T With<T>(this T o, params object[] pattern) => o;
		
		public static T With<T>(this T o) => o;
		public static T With<T, A>(this T o, A a) => o;
		public static T With<T, A, B>(this T o, A a, B b) => o;
		public static T With<T, A, B, C>(this T o, A a, B b, C c) => o;
		public static T With<T, A, B, C, D>(this T o, A a, B b, C c, D d) => o;
		public static T With<T, A, B, C, D, E>(this T o, A a, B b, C c, D d, E e) => o;
		public static T With<T, A, B, C, D, E, F>(this T o, A a, B b, C c, D d, E e, F f) => o;
		public static T With<T, A, B, C, D, E, F, G>(this T o, A a, B b, C c, D d, E e, F f, G g) => o;
		public static T With<T, A, B, C, D, E, F, G, H>(this T o, A a, B b, C c, D d, E e, F f, G g, H h) => o;
		
		public const object Null = null;
		public static object With() => Null.With();
		
		// ReSharper enable InconsistentNaming

		public static TCollection Merge<TCollection, T>(this TCollection collection, IOrderedEnumerable<T> items)
			where TCollection : ICollection<T>
		{
			items.ForEach(collection.Add); // foreach (var item in items) collection.Add(item);
			return collection;
		}

		public static TCollection Merge<TCollection, T>(this TCollection collection, IEnumerable<T> items)
			where TCollection : ICollection<T>
		{
			items.ForEach(collection.Add); // foreach (var item in items) collection.Add(item);
			return collection;
		}
		
		public static TCollection Merge<TCollection, T>(this TCollection collection, params T[] items)
			where TCollection : ICollection<T>
		{
			items.ForEach(collection.Add); // foreach (var item in items) collection.Add(item);
			return collection;
		}

		public static bool IsTrue(this bool b) => b;
		public static bool IsFalse(this bool b) => !b;
		
		public static bool All(this bool[] conditions, bool value) => value ? conditions.All(IsTrue) : conditions.All(IsFalse);
		public static bool Any(this bool[] conditions, bool value) => value ? conditions.Any(IsTrue) : conditions.Any(IsFalse);
		public static int Count(this bool[] pattern, bool value) => value ? pattern.Count(IsTrue) : pattern.Count(IsFalse);
		public static bool[] Check<T>(this T o, params bool[] pattern) => pattern;

		public static KeyValuePair<TK, TV> To<TK, TV>(this TK key, TV value) => new KeyValuePair<TK, TV>(key, value);
		public static KeyValuePair<TK, TV> By<TK, TV>(this TV value, TK key) => new KeyValuePair<TK, TV>(key, value);

		#region LambdaStyledMatching

		// ReSharper disable InconsistentNaming
		public static TOut Match<TIn, TOut>(this TIn context, Func<TOut> nullCase = null)
			=> context.InvokeMatcher((Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null,
				(Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A>(this TIn context,
			Func<A, TOut> a, Func<TOut> nullCase = null)
			where A : TIn
			=> context.InvokeMatcher(a, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null,
				(Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<TOut> nullCase = null)
			where A : TIn where B : TIn
			=> context.InvokeMatcher(a, b, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null,
				(Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn
			=> context.InvokeMatcher(a, b, c, (Func<TIn, TOut>) null,
				(Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C, D>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn
			=> context.InvokeMatcher(a, b, c, d,
				(Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C, D, E>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d,
			Func<E, TOut> e, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn
			=> context.InvokeMatcher(a, b, c, d, e, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C, D, E, F>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d,
			Func<E, TOut> e, Func<F, TOut> f, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn
			=> context.InvokeMatcher(a, b, c, d, e, f, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C, D, E, F, G>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d,
			Func<E, TOut> e, Func<F, TOut> f, Func<G, TOut> g, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn where G : TIn
			=> context.InvokeMatcher(a, b, c, d, e, f, g, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C, D, E, F, G, H>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d,
			Func<E, TOut> e, Func<F, TOut> f, Func<G, TOut> g, Func<H, TOut> h, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn where G : TIn where H : TIn
			=> context.InvokeMatcher(a, b, c, d, e, f, g, h, nullCase);
		
		private static TOut InvokeMatcher<TIn, TOut, A, B, C, D, E, F, G, H>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d,
			Func<E, TOut> e, Func<F, TOut> f, Func<G, TOut> g, Func<H, TOut> h, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn where G : TIn where H : TIn
		{
			switch (context)
			{
				case A aa when a != null: return a.Invoke(aa);
				case B bb when b != null: return b.Invoke(bb);
				case C cc when c != null: return c.Invoke(cc);
				case D dd when d != null: return d.Invoke(dd);
				case E ee when e != null: return e.Invoke(ee);
				case F ff when f != null: return f.Invoke(ff);
				case G gg when g != null: return g.Invoke(gg);
				case H hh when h != null: return h.Invoke(hh);
				default:
					return nullCase != null && context.IsNull()
						? nullCase.Invoke()
						: throw new ArgumentException($"Undefined case for '{context}'");
			}
		}
		// ReSharper enable InconsistentNaming
		
		#endregion
		
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
		public static int[] IndexesOf<T>(this IEnumerable<T> collection, T value)
		{
			var indexes = new List<int>();
			var i = 0;
			foreach (var item in collection)
			{
				if (item.Is(value)) indexes.Add(i);
				i++;
			}
			
			return indexes.ToArray();
		}
		
		public static int ClearFrom<T>(this IList<T> collection, T value)
		{
			var indexes = collection.IndexesOf(value);
			for (var i = indexes.Length - 1; i >= 0; i--)
			{
				collection.RemoveAt(indexes[i]);
			}
			
			return indexes.Length;
		}

		public static IList<T> Trim<T>(this IList<T> collection, int index)
		{
			for (var i = collection.Count - 1; i >= index; i--)
			{
				collection.RemoveAt(i);
			}

			return collection;
		}

		public static IList<T> ForEach<T>(this IList<T> collection, Action<T> action)
		{
			foreach (var item in collection) action(item);
			return collection;
		}

		public static IList<T> ForEach<T, TR>(this IList<T> collection, Func<T, TR> action)
		{
			foreach (var item in collection) action(item);
			return collection;
		}

		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach (var item in collection) action(item);
		}
		
		public static void ForEach<T, TR>(this IEnumerable<T> collection, Func<T, TR> action)
		{
			foreach (var item in collection) action(item);
		}

		public static IEnumerable<T> Cast<T>(this IDictionary dictionary)
		{
			foreach (T item in dictionary) yield return item;
		}

		public static IEnumerable<T> ToEnumerable<T>(this T singleItem) { yield return singleItem; }

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