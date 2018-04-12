using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable once InconsistentNaming
	public static partial class LE
	{
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
	}
}