using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ace.Comparers
{
	public class ReferenceComparer<T> : IEqualityComparer<T>
	{
		public static readonly ReferenceComparer<T> Default = new();
		
		public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
		
		public bool Equals(T x, T y) => ReferenceEquals(x, y);
	}
}
