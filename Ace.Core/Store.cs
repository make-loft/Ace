using System;
using System.Collections.Generic;
using System.Linq;
using Ace.Sugar;

namespace Ace
{
	public static class Store
	{	
		private static readonly Dictionary<Type, object> Container = new Dictionary<Type, object>();

		public static object Get(Type type, params object[] cctorArgs) =>
			Lock.Invoke(Container, _ => Container.TryGetValue(type, out var item) ? item : Revive(type, cctorArgs));

		public static TItem Get<TItem>(params object[] cctorArgs) where TItem : class =>
			(TItem) Get(TypeOf<TItem>.Raw, cctorArgs);

		public static void Set<TItem>(TItem value) where TItem : class => Container[TypeOf<TItem>.Raw] = value;

		public static void Snapshot() => Container.Values.ForEach(i => Memory.ActiveBox.Keep(i));

		internal static object Revive(Type type, params object[] constructorArgs) =>
			Memory.ActiveBox.Revive(null, type, constructorArgs)
				.Use(item => Container.Add(type, item)) /* note: Add before Expose */
				.Use(item => item.As<IExposable>()?.Expose());
	}
}