using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable once InconsistentNaming
	public static partial class LE
	{
		public static TCollection Merge<TCollection, TElement>(this TCollection collection, IEnumerable<TElement> items)
			where TCollection : ICollection<TElement>
		{
			items.ForEach(collection.Add); // foreach (var item in items) collection.Add(item);
			return collection;
		}

		public static TCollection Merge<TCollection, TElement>(this TCollection collection, params TElement[] items)
			where TCollection : ICollection<TElement> => items.ForEach(collection.Add).Put(collection);
	}
}