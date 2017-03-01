using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Art.Replication
{
    static class Adapters
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}
