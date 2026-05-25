using System;
using System.Collections.Generic;

namespace Ace;

public static partial class New
{
	public static T Lazy<T>(ref T o) where T : class, new() => o ??= new();
	public static T Lazy<T>(ref T o, params object[] args) where T : class, new() => o ??= Object<T>(args);
	
	public static T _<T>(out T o) where T : new() => o = new T();

	public static T _<T>(out T o, params object[] constructorArgs)
		=> o = (T) Activator.CreateInstance(TypeOf<T>.Raw, constructorArgs);
	
	public static T Object<T>() where T : new() => new();

	public static T Object<T>(params object[] constructorArgs)
		=> (T) Activator.CreateInstance(TypeOf<T>.Raw, constructorArgs);

	public static object Object(Type type, params object[] constructorArgs)
		=> Activator.CreateInstance(type, constructorArgs);

	public static T[] Array<T>(params T[] items) => items;

	public static List<T> List<T>(int capacity = 0) => new(capacity);
	public static List<T> List<T>(params T[] items) => List<T>(items.Length).AppendRange(items);

	public static Dictionary<TK, TV> Dictionary<TK, TV>() => [];

	public static Dictionary<TK, TV> Dictionary<TK, TV>(params KeyValuePair<TK, TV>[] items)
		=> new Dictionary<TK, TV>(items.Length).AppendRange(items);


	public static EventHandler EventHandler(EventHandler h) => h;
	public static EventHandler<T> EventHandler<T>(EventHandler<T> h) where T : EventArgs => h;
	
	public static Func<_> Func<_>(Func<_> f) => f;
	public static Func<A, _> Func<A, _>(Func<A, _> f) => f;
	public static Func<A, B, _> Func<A, B, _>(Func<A, B, _> f) => f;
	public static Func<A, B, C, _> Func<A, B, C, _>(Func<A, B, C, _> f) => f;
	public static Func<A, B, C, D, _> Func<A, B, C, D, _>(Func<A, B, C, D, _> f) => f;
	public static Func<A, B, C, D, E, _> Func<A, B, C, D, E, _>(Func<A, B, C, D, E, _> f) => f;
	public static Func<A, B, C, D, E, F, _> Func<A, B, C, D, E, F, _>(Func<A, B, C, D, E, F, _> f) => f;
	public static Func<A, B, C, D, E, F, G, _> Func<A, B, C, D, E, F, G, _>(Func<A, B, C, D, E, F, G, _> f) => f;
	public static Func<A, B, C, D, E, F, G, H, _> Func<A, B, C, D, E, F, G, H, _>(Func<A, B, C, D, E, F, G, H, _> f) => f;

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