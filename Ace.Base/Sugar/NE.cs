﻿using System;
using System.Collections.Generic;
// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable once InconsistentNaming
	/* Short forms of New */
	public static class NE
	{
		public static T O<T>() where T : new() => new T();

		public static T O<T>(params object[] constructorArgs) =>
			(T) Activator.CreateInstance(TypeOf<T>.Info, constructorArgs);

		public static object O(Type type, params object[] constructorArgs) =>
			Activator.CreateInstance(type, constructorArgs);

		public static T[] A<T>(params T[] items) => items;

		public static List<T> L<T>(params T[] items) => new List<T>(items);

		public static Dictionary<TK, TV> D<TK, TV>(params KeyValuePair<TK, TV>[] items) =>
			new Dictionary<TK, TV>().Merge(items);
	}
}