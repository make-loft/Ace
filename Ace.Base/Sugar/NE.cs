using System;
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
			(T) Activator.CreateInstance(TypeOf<T>.Raw, constructorArgs);

		public static object O(Type type, params object[] constructorArgs) =>
			Activator.CreateInstance(type, constructorArgs);

		public static T[] A<T>(params T[] items) => items;

		public static List<T> L<T>(int capacity) => new List<T>(capacity);
		public static List<T> L<T>(params T[] items) => L<T>(items.Length).MergeMany(items);

		public static List<T> L<T>(T a) => L<T>(1).Merge(a);
		public static List<T> L<T>(T a, T b) => L<T>(2).Merge(a, b);
		public static List<T> L<T>(T a, T b, T c) => L<T>(3).Merge(a, b, c);
		public static List<T> L<T>(T a, T b, T c, T d) => L<T>(4).Merge(a, b, c, d);
		public static List<T> L<T>(T a, T b, T c, T d, T e) => L<T>(5).Merge(a, b, c, d, e);
		public static List<T> L<T>(T a, T b, T c, T d, T e, T f) => L<T>(6).Merge(a, b, c, d, e, f);
		public static List<T> L<T>(T a, T b, T c, T d, T e, T f, T g) => L<T>(7).Merge(a, b, c, d, e, f, g);
		public static List<T> L<T>(T a, T b, T c, T d, T e, T f, T g, T h) => L<T>(8).Merge(a, b, c, d, e, f, g, h);

		public static Dictionary<TK, TV> D<TK, TV>() => new Dictionary<TK, TV>();

		public static Dictionary<TK, TV> D<TK, TV>(params KeyValuePair<TK, TV>[] items) =>
			new Dictionary<TK, TV>().MergeMany(items);

		public static Dictionary<TK, TV> D<TK, TV>(
			KeyValuePair<TK, TV> a) =>
			D<TK, TV>().Merge(a);

		public static Dictionary<TK, TV> D<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b) =>
			D<TK, TV>().Merge(a, b);

		public static Dictionary<TK, TV> D<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c) =>
			D<TK, TV>().Merge(a, b, c);

		public static Dictionary<TK, TV> D<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c, KeyValuePair<TK, TV> d) =>
			D<TK, TV>().Merge(a, b, c, d);

		public static Dictionary<TK, TV> D<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c, KeyValuePair<TK, TV> d,
			KeyValuePair<TK, TV> e) =>
			D<TK, TV>().Merge(a, b, c, d, e);

		public static Dictionary<TK, TV> D<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c, KeyValuePair<TK, TV> d,
			KeyValuePair<TK, TV> e, KeyValuePair<TK, TV> f) =>
			D<TK, TV>().Merge(a, b, c, d, e, f);

		public static Dictionary<TK, TV> D<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c, KeyValuePair<TK, TV> d,
			KeyValuePair<TK, TV> e, KeyValuePair<TK, TV> f, KeyValuePair<TK, TV> g) =>
			D<TK, TV>().Merge(a, b, c, d, e, f, g);

		public static Dictionary<TK, TV> D<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c, KeyValuePair<TK, TV> d,
			KeyValuePair<TK, TV> e, KeyValuePair<TK, TV> f, KeyValuePair<TK, TV> g, KeyValuePair<TK, TV> h) =>
			D<TK, TV>().Merge(a, b, c, d, e, f, g, h);
	}
}