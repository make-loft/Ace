using System;
using System.Collections.Generic;
using System.Linq;
using Ace.Sugar;

namespace Ace
{
	public static class Store
	{
		public static readonly Dictionary<Type, object> Container = new Dictionary<Type, object>();

		public static object Get(Type type, params object[] cctorArgs) => Container.TryGetValue(type, out var item)
			? item
			: Lock.Invoke(() => Container.TryGetValue(type, out item) ? item : Revive(type, cctorArgs));

		public static TItem Get<TItem>(params object[] cctorArgs) where TItem : class =>
			(TItem) Get(TypeOf<TItem>.Raw, cctorArgs);

		public static void Set<TItem>(TItem value) where TItem : class => Container[TypeOf<TItem>.Raw] = value;

		public static void Snapshot() => Container.Values.ForEach(i => Memory.ActiveBox.Keep(i));

		internal static object Revive(Type type, params object[] constructorArgs) =>
			Memory.ActiveBox.Revive(null, type, constructorArgs)
				.Use(item => Container.Add(type, item)) /* note: Add before Expose */
				.Use(item => (item as IExposable)?.Expose());
	}

	public static class Store<TItem> where TItem : class
	{
		private static TItem _item;

		public static TItem Set(TItem item) => _item = item;

		public static TItem Get(params object[] cctorArgs) =>
			_item ?? Lock.Invoke(() => _item ?? Store.Revive(TypeOf<TItem>.Raw, cctorArgs).To(out _item));

	}
}