using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable once InconsistentNaming
	public static partial class LE
	{
		public static T UncheckedAddTo<T>(this T item, IList list)
		{
			list.Add(item);
			return item;
		}

		public static T AddTo<T>(this T item, ICollection<T> collecton)
		{
			collecton.Add(item);
			return item;
		}

		public static TCollection MergeMany<TCollection, TElement>(this TCollection collection, IEnumerable<TElement> items)
			where TCollection : ICollection<TElement>
		{
			items.ForEach(collection.Add); // foreach (var item in items) collection.Add(item);
			return collection;
		}

		//public static TCollection Merge<TCollection, TElement>(this TCollection collection, params TElement[] items)
		//	where TCollection : ICollection<TElement> => items.ForEach(collection.Add).Put(collection);

		public static TCollection Merge<TCollection, T>(this TCollection collection)
			where TCollection : ICollection<T> => collection;

		public static TCollection Merge<TCollection, T>(this TCollection collection, T a)
			where TCollection : ICollection<T> 
		{
			collection.Add(a);
			return collection;
		}

		public static TCollection Merge<TCollection, T>(this TCollection collection, T a, T b)
			where TCollection : ICollection<T> =>
			collection.Merge(a).Merge(b);
		
		public static TCollection Merge<TCollection, T>(this TCollection collection, T a, T b, T c)
			where TCollection : ICollection<T> =>
			collection.Merge(a, b).Merge(c);
		
		public static TCollection Merge<TCollection, T>(this TCollection collection, T a, T b, T c, T d)
			where TCollection : ICollection<T> =>
			collection.Merge(a, b, c).Merge(d);
		
		public static TCollection Merge<TCollection, T>(this TCollection collection, T a, T b, T c, T d, T e)
			where TCollection : ICollection<T> =>
			collection.Merge(a, b, c, d).Merge(e);

		public static TCollection Merge<TCollection, T>(this TCollection collection, T a, T b, T c, T d, T e, T f)
			where TCollection : ICollection<T> =>
			collection.Merge(a, b, c, d, e).Merge(f);
		
		public static TCollection Merge<TCollection, T>(this TCollection collection, T a, T b, T c, T d, T e, T f, T g)
			where TCollection : ICollection<T> =>
			collection.Merge(a, b, c, d, e, f).Merge(g);
		
		public static TCollection Merge<TCollection, T>(this TCollection collection, T a, T b, T c, T d, T e, T f, T g, T h)
			where TCollection : ICollection<T> =>
			collection.Merge(a, b, c, d, e, f, g).Merge(h);
	}
}