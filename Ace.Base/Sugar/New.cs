using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Ace
{
	public static partial class New
	{
		public static T Object<T>() where T : new() => new T();

		public static T Object<T>(params object[] constructorArgs) =>
			(T) Activator.CreateInstance(TypeOf<T>.Raw, constructorArgs);

		public static object Object(Type type, params object[] constructorArgs) =>
			Activator.CreateInstance(type, constructorArgs);

		public static T[] Array<T>(params T[] items) => items;

		public static List<T> List<T>(int capacity = 0) => new List<T>(capacity);
		public static List<T> List<T>(params T[] items) => List<T>(items.Length).MergeMany(items);

		public static List<T> List<T>(T a) => List<T>(1).Merge(a);
		public static List<T> List<T>(T a, T b) => List<T>(2).Merge(a, b);
		public static List<T> List<T>(T a, T b, T c) => List<T>(3).Merge(a, b, c);
		public static List<T> List<T>(T a, T b, T c, T d) => List<T>(4).Merge(a, b, c, d);
		public static List<T> List<T>(T a, T b, T c, T d, T e) => List<T>(5).Merge(a, b, c, d, e);
		public static List<T> List<T>(T a, T b, T c, T d, T e, T f) => List<T>(6).Merge(a, b, c, d, e, f);
		public static List<T> List<T>(T a, T b, T c, T d, T e, T f, T g) => List<T>(7).Merge(a, b, c, d, e, f, g);
		public static List<T> List<T>(T a, T b, T c, T d, T e, T f, T g, T h) => List<T>(8).Merge(a, b, c, d, e, f, g, h);

		public static Dictionary<TK, TV> Dictionary<TK, TV>() => new Dictionary<TK, TV>();

		public static Dictionary<TK, TV> Dictionary<TK, TV>(params KeyValuePair<TK, TV>[] items) =>
			new Dictionary<TK, TV>().MergeMany(items);

		public static Dictionary<TK, TV> Dictionary<TK, TV>(
			KeyValuePair<TK, TV> a) =>
			Dictionary<TK, TV>().Merge(a);

		public static Dictionary<TK, TV> Dictionary<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b) =>
			Dictionary<TK, TV>().Merge(a, b);

		public static Dictionary<TK, TV> Dictionary<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c) =>
			Dictionary<TK, TV>().Merge(a, b, c);

		public static Dictionary<TK, TV> Dictionary<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c, KeyValuePair<TK, TV> d) =>
			Dictionary<TK, TV>().Merge(a, b, c, d);

		public static Dictionary<TK, TV> Dictionary<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c, KeyValuePair<TK, TV> d,
			KeyValuePair<TK, TV> e) =>
			Dictionary<TK, TV>().Merge(a, b, c, d, e);

		public static Dictionary<TK, TV> Dictionary<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c, KeyValuePair<TK, TV> d,
			KeyValuePair<TK, TV> e, KeyValuePair<TK, TV> f) =>
			Dictionary<TK, TV>().Merge(a, b, c, d, e, f);

		public static Dictionary<TK, TV> Dictionary<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c, KeyValuePair<TK, TV> d,
			KeyValuePair<TK, TV> e, KeyValuePair<TK, TV> f, KeyValuePair<TK, TV> g) =>
			Dictionary<TK, TV>().Merge(a, b, c, d, e, f, g);

		public static Dictionary<TK, TV> Dictionary<TK, TV>(
			KeyValuePair<TK, TV> a, KeyValuePair<TK, TV> b, KeyValuePair<TK, TV> c, KeyValuePair<TK, TV> d,
			KeyValuePair<TK, TV> e, KeyValuePair<TK, TV> f, KeyValuePair<TK, TV> g, KeyValuePair<TK, TV> h) =>
			Dictionary<TK, TV>().Merge(a, b, c, d, e, f, g, h);
	}
}