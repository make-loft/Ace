using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Ace
{
	public static partial class New
	{	
		public static T Object<T>() where T : new() => new T();

		public static T Object<T>(params object[] constructorArgs) =>
			(T) Activator.CreateInstance(typeof(T), constructorArgs);

		public static object Object(Type type, params object[] constructorArgs) =>
			Activator.CreateInstance(type, constructorArgs);
		
		public static T[] Array<T>(params T[] items) => items;
		
		public static List<T> List<T>(params T[] items) => new List<T>(items);

		public static Dictionary<TK, TV> Dictionary<TK, TV>(params KeyValuePair<TK, TV>[] items) =>
			new Dictionary<TK, TV>().Merge(items);
	}
}