using System.Collections;
using System.Collections.Generic;
using Ace;

// ReSharper disable once CheckNamespace
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
			items.ToDictionary(Pair<TKey, TValue>.Key, Pair<TKey, TValue>.Value);

		public static Dictionary<TValue, TKey>
			ToMirrorDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items) =>
			items.ToDictionary(Pair<TKey, TValue>.Value, Pair<TKey, TValue>.Key);
		
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
		
		internal static IEnumerable<object> EnumerateMultidimensionArray(this IEnumerable items, int rank)
		{
			foreach (var item in items)
			{
				if (item is IEnumerable s && rank > 0)
				{
					var subitems = EnumerateMultidimensionArray(s, rank - 1);
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