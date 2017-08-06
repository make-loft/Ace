using System;
using System.Collections.Generic;
using System.Linq;

namespace Art
{
    public static class Store
    {
        public static readonly Dictionary<Type, object> Container = new Dictionary<Type, object>();

        public static TItem Get<TItem>(params object[] constructorArgs) where TItem : class => 
            Container.TryGetValue(typeof(TItem), out var item) ? (TItem) item : Revive<TItem>(constructorArgs);

        public static void Set<TItem>(TItem value) where TItem : class => Container[typeof(TItem)] = value;

        public static void Snapshot() => Container.Values.ForEach(i => Memory.ActiveBox.Keep(i));
        
        private static TItem Revive<TItem>(params object[] constructorArgs) where TItem : class
        {
            var item = Memory.ActiveBox.Revive<TItem>(null, constructorArgs);
            Container.Add(typeof(TItem), item); // important: add item before Expose call
            (item as IExposable)?.Expose();
            return item;
        }
    }
}