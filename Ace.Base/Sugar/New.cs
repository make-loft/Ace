using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Ace
{
	public static partial class New
	{
		public static T _<T>(out T o) where T : new() => o = new T();

		public static T _<T>(out T o, params object[] constructorArgs) =>
			o = (T) Activator.CreateInstance(TypeOf<T>.Raw, constructorArgs);
		
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

		public static EventHandler EventHandler(EventHandler h) => h;
		public static EventHandler<T> EventHandler<T>(EventHandler<T> h) where T : EventArgs => h;
		
		public static Func<Z> Func<Z>(Func<Z> f) => f;
		public static Func<A, Z> Func<A, Z>(Func<A, Z> f) => f;
		public static Func<A, B, Z> Func<A, B, Z>(Func<A, B, Z> f) => f;
		public static Func<A, B, C, Z> Func<A, B, C, Z>(Func<A, B, C, Z> f) => f;
		public static Func<A, B, C, D, Z> Func<A, B, C, D, Z>(Func<A, B, C, D, Z> f) => f;
		public static Func<A, B, C, D, E, Z> Func<A, B, C, D, E, Z>(Func<A, B, C, D, E, Z> f) => f;
		public static Func<A, B, C, D, E, F, Z> Func<A, B, C, D, E, F, Z>(Func<A, B, C, D, E, F, Z> f) => f;
		public static Func<A, B, C, D, E, F, G, Z> Func<A, B, C, D, E, F, G, Z>(Func<A, B, C, D, E, F, G, Z> f) => f;
		public static Func<A, B, C, D, E, F, G, H, Z> Func<A, B, C, D, E, F, G, H, Z>(Func<A, B, C, D, E, F, G, H, Z> f) => f;

		public static Action Action(Action a) => a;
		public static Action<A> Action<A>(Action<A> a) => a;
		public static Action<A, B> Action<A, B>(Action<A, B> a) => a;
		public static Action<A, B, C> Action<A, B, C>(Action<A, B, C> a) => a;
		public static Action<A, B, C, D> Action<A, B, C, D>(Action<A, B, C, D> a) => a;
		public static Action<A, B, C, D, E> Action<A, B, C, D, E>(Action<A, B, C, D, E> a) => a;
		public static Action<A, B, C, D, E, F> Action<A, B, C, D, E, F>(Action<A, B, C, D, E, F> a) => a;
		public static Action<A, B, C, D, E, F, G> Action<A, B, C, D, E, F, G>(Action<A, B, C, D, E, F, G> a) => a;
		public static Action<A, B, C, D, E, F, G, H> Action<A, B, C, D, E, F, G, H>(Action<A, B, C, D, E, F, G, H> a) => a;
		public static Action<A, B, C, D, E, F, G, H, I> Action<A, B, C, D, E, F, G, H, I>(Action<A, B, C, D, E, F, G, H, I> a) => a;
	}
}