﻿using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace, InconsistentNaming
namespace Ace
{
	/* The Language Extensions */
	public static partial class LE
	{
		/* {oldContext}.Put({newContext}).{CallFromNewContext} */
		public static TX Put<T, TX>(this T o, TX x) => x;
		public static ref TX Put<T, TX>(this T o, ref TX x) => ref x;

		public static T Let<T, TX>(this T o, out TX x, TX value = default) => (x = value).Put(o);
		public static ref T Let<T, TX>(this ref T o, out TX x, TX value = default) where T : struct
		{
			x = value;
			return ref o;
		}

		/* bool */
		public static bool Not(this in bool b) => !b;
		public static bool IsTrue(this in bool b) => b;
		public static bool IsFalse(this in bool b) => !b;

		/* boxing free value matching */
		public static bool Equals<T>(T a, T b) => EqualityComparer<T>.Default.Equals(a, b);

		public static bool Is<T>(this T o) => o != null;
		public static bool Is<T>(this T o, T x) => Equals(o, x); /* Equals<T>(o, x); */
		public static bool Is<T>(this T o, object x) => x.Is<T>() && Equals(o, (T)x);
		public static bool Is<T>(this object o, T x) => o.Is<T>() && Equals((T)o, x);
		public static bool Is(this object o, object x) => Equals(o, x);

		public static bool IsNot<T>(this T o) => o == null;
		public static bool IsNot<T>(this T o, T x) => !o.Is(x);
		public static bool IsNot<T>(this object o, T x) => !o.Is(x);
		public static bool IsNot<T>(this T o, object x) => !o.Is(x);
		public static bool IsNot(this object o, object x) => !o.Is(x);

		public static bool Is<T>(this T? o) where T : struct => o.HasValue; /* o is T */
		public static bool Is<T>(this T? o, T x) where T : struct => o.HasValue && Equals(o.Value, x);
		public static bool Is<T>(this T o, T? x) where T : struct => x.HasValue && Equals(o, x.Value);

		public static bool IsNot<T>(this T? o) where T : struct => !o.Is();
		public static bool IsNot<T>(this T? o, T x) where T : struct => !o.Is(x);
		public static bool IsNot<T>(this T o, T? x) where T : struct => !o.Is(x);

		/* string based value matching */
		public static int Compare(this string o, string x, StringComparison comparison = StringComparison.Ordinal) =>
			string.Compare(o, x, comparison);

		public static bool Is(this string o, string x, StringComparison comparison) => o.Compare(x, comparison).Is(0);
		public static bool Is(this object o, object x, StringComparison comparison) => o.Is(x) || o.To<string>().Is(x.To<string>(), comparison);
		public static bool IsNot(this string o, string x, StringComparison comparison) => !o.Is(x, comparison);
		public static bool IsNot(this object o, object x, StringComparison comparison) => !o.Is(x, comparison);

		/* type matching */
		public static bool Is<T>(this object o) => o is T; /* o != null && typeof(T).IsAssignableFrom(o.GetType());	*/
		public static bool IsNot<T>(this object o) => !o.Is<T>();

		public static bool Is<T>(this T o, out T x) => (x = o).Is();
		public static bool Is<T>(this object o, out T x, T fallbackValue = default) =>
			(x = o.Is<T>().To(out var b) ? (T)o : fallbackValue).Put(ref b);

		/* type casting */
		public static object ChangeType<T>(this object o) => TypeOf<string>.Raw.Is(TypeOf<T>.Raw)
			? o?.ToString()
			: o is null || TypeOf<T>.IsValueType || o is IConvertible
				? Convert.ChangeType(o, TypeOf<T>.Raw, null)
				: o;

		public static T To<T>(this T o) => o;
		public static T To<T>(this object o) => (T)o.ChangeType<T>();
		public static ref T To<T>(this T o, out T x) { x = o; return ref x; }
		public static ref T To<T>(this object o, out T x) { x = (T)o.ChangeType<T>(); return ref x; }
		public static ref T? To<T>(this object o, out T? x) where T : struct { x = (T?)o; return ref x; }

		public static T As<T>(this T o) => o;
		public static T As<T>(this T o, out T x) => x = o;
		public static T As<T>(this object o, T fallbackValue = default) => o.Is<T>() ? (T)o : fallbackValue;
		public static T As<T>(this object o, out T x, T fallbackValue = default) => x = o.As(fallbackValue);
	}
}