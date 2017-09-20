using System;
using System.Collections.Generic;
using System.Linq;

namespace Ace
{
    public static class Store
    {
        public static readonly Dictionary<Type, object> Container = new Dictionary<Type, object>();

        public static object Get(Type type, params object[] constructorArgs) =>
            Container.TryGetValue(type, out var item) ? item : Revive(type, constructorArgs);

        public static TItem Get<TItem>(params object[] constructorArgs) where TItem : class =>
            (TItem) Get(typeof(TItem), constructorArgs);

        public static void Set<TItem>(TItem value) where TItem : class => Container[typeof(TItem)] = value;

        public static void Snapshot() => Container.Values.ForEach(i => Memory.ActiveBox.Keep(i));
        
        private static object Revive(Type type, params object[] constructorArgs)
        {
            var item = Memory.ActiveBox.Revive(null, type, constructorArgs);
            Container.Add(type, item); // important: add item before Expose call
            (item as IExposable)?.Expose();
            return item;
        }
    }
}