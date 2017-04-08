using System;
using System.Collections.Generic;
using System.Linq;

namespace Art
{
    public static class Store
    {
        public static readonly Dictionary<Type, object> Container = new Dictionary<Type, object>();

        public static TItem Get<TItem>(params object[] constructorArgs) where TItem : class
        {
            var itemType = typeof (TItem);
            if (Container.TryGetValue(itemType, out var item)) 
                return (TItem) item;

            var newItem = Memory.ActiveBox.Revive<TItem>(null, constructorArgs);
            Container.Add(itemType, newItem);
            (newItem as IExposable)?.Expose();
            return newItem;
        }

        public static void Set<TItem>(TItem value) where TItem : class
        {
            var itemType = typeof (TItem);
            if (!Container.ContainsKey(itemType))
                Container.Add(itemType, value);
            else Container[itemType] = value;
        }

        public static void Snapshot() => Container.Values.ForEach(i => Memory.ActiveBox.Keep(i));
    }
}