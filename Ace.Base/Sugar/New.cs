﻿using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
// ReSharper disable InconsistentNaming
namespace Ace
{
	public static partial class New
	{
		public static T Lazy<T>(ref T o) where T : class, new() => o ??= new();
		public static T Lazy<T>(ref T o, params object[] args) where T : class, new() => o ??= Object<T>(args);
		
		public static T _<T>(out T o) where T : new() => o = new T();

		public static T _<T>(out T o, params object[] constructorArgs) =>
			o = (T) Activator.CreateInstance(TypeOf<T>.Raw, constructorArgs);
		
		public static T Object<T>() where T : new() => new();

		public static T Object<T>(params object[] constructorArgs) =>
			(T) Activator.CreateInstance(TypeOf<T>.Raw, constructorArgs);

		public static object Object(Type type, params object[] constructorArgs) =>
			Activator.CreateInstance(type, constructorArgs);

		public static T[] Array<T>(params T[] items) => items;

		public static List<T> List<T>(int capacity = 0) => new(capacity);
		public static List<T> List<T>(params T[] items) => List<T>(items.Length).AppendRange(items);

		public static Dictionary<TK, TV> Dictionary<TK, TV>() => new();

		public static Dictionary<TK, TV> Dictionary<TK, TV>(params KeyValuePair<TK, TV>[] items) =>
			new Dictionary<TK, TV>(items.Length).AppendRange(items);


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
	}
}