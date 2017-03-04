using System;
using System.Collections.Generic;

namespace Aero
{
    public static class Store
    {
        public static readonly Dictionary<Type, object> Container = new Dictionary<Type, object>();

        public static TItem Get<TItem>(params object[] constructorArgs) where TItem : class
        {
            var itemType = typeof (TItem);
            if (Container.ContainsKey(itemType)) 
                return (TItem) Container[itemType];

            var item = Memory.ActiveBox.Revive<TItem>(null, constructorArgs);
            Container.Add(itemType, item);
            var exposable = item as IExposable;
            exposable?.Expose();
            return item;
        }

        public static void Set<TItem>(TItem value) where TItem : class
        {
            var itemType = typeof (TItem);
            if (!Container.ContainsKey(itemType))
                Container.Add(itemType, value);
            else Container[itemType] = value;
        }

        public static void Snapshot()
        {
            Container.Values.ForEach(i => Memory.ActiveBox.Keep(i));
        }
    }
}