using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Art.Comparers
{
    public class ReferenceComparer<T> : IEqualityComparer<T>
    {
        public static readonly ReferenceComparer<T> Default = new ReferenceComparer<T>();
        
        public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
        
        public bool Equals(T x, T y) => ReferenceEquals(x, y);
    }
}
