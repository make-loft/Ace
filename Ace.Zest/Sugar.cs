using System.Collections.Generic;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
	public static class EnumerableArt
	{
		public static async void ForEachAsync<T>(this IEnumerable<T> collection, Func<T, Task> action)
		{
			foreach (var item in collection) await action(item);
		}
		
		public static async void ForEachAsync<T, TR>(this IEnumerable<T> collection, Func<T, Task<TR>> action)
		{
			foreach (var item in collection) await action(item);
		}
	}
}