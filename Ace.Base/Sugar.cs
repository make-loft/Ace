using System;
using System.Collections.Generic;

namespace Ace
{
	// ReSharper disable once InconsistentNaming
	/* LanguageExtensions */
	public static partial class LE
	{
		private static object ChangeType(this object o, Type type) =>
			o == null || type.IsValueType || o is IConvertible ? Convert.ChangeType(o, type, null) : o;

		public static T To<T>(this T o) => o;
		public static T To<T>(this T o, out T x) => x = o;
		public static T To<T>(this object o) => (T) ChangeType(o, typeof(T));
		public static T To<T>(this object o, out T x) => x = (T) ChangeType(o, typeof(T));

		public static TR Like<T, TR>(this T o, TR y) => y;
		public static TR Like<T, TR>(this T o, ref TR y) => y;
		public static TR Like<T, TR>(this T o, TR y, out T x) => (x = o).Like(y);
		public static TR Like<T, TR>(this T o, ref TR y, out T x) => (x = o).Like(y);

		public static T As<T>(this T o) => o;
		public static T As<T>(this T o, out T x) => x = o;
		public static T As<T>(this object o, T fallbackValue = default(T)) => o is T ? (T) o : fallbackValue;
		public static T As<T>(this object o, out T x, T fallbackValue = default(T)) => x = o.As(fallbackValue);

		public static bool Is<T>(this T o) => typeof(T).IsValueType || o != null; // o is T
		public static bool Is<T>(this T? o) where T : struct => o.HasValue; // o is T
		public static bool Is<T>(this T o, out T x) => (x = o).Is();
		public static bool Is<T>(this object o) => o is T; // o != null && typeof(T).IsAssignableFrom(o.GetType());	

		public static bool Is<T>(this object o, out T x, T fallbackValue = default(T)) =>
			(x = o.Is<T>().To(out var b) ? (T) o : fallbackValue).Like(b);

		public static bool IsNull<T>(this T o) => !typeof(T).IsValueType && o == null; /* is null */
		public static bool IsNull<T>(this T? o) where T : struct => !o.HasValue; /* is null */
		public static bool IsNull<T>(this T o, out T x) => (x = o).IsNull();
		public static bool IsNull(this object o) => o is null;

		public static bool IsNull<T>(this object o, out T x, T fallbackValue = default(T)) =>
			(x = o.IsNull().To(out var b) ? (T) o : fallbackValue).Like(b);

		public static bool Equals<T>(T a, T b) => EqualityComparer<T>.Default.Equals(a, b); /* boxing free equals */

		public static bool Is<T>(this T o, T x) => Equals(o, x); /* Equals<T>(o, x); */
		public static bool Is<T>(this T o, object x) => x.Is<T>() && Equals(o, (T) x);
		public static bool Is<T>(this object o, T x) => o.Is<T>() && Equals((T) o, x);
		public static bool Is<T>(this T? o, T x) where T : struct => o.HasValue && Equals(o.Value, x);
		public static bool Is<T>(this T o, T? x) where T : struct => x.HasValue && Equals(o, x.Value);

		public static bool IsNot<T>(this T o, T x) => !o.Is(x);
		public static bool IsNot<T>(this object o, T x) => !o.Is(x);
		public static bool IsNot<T>(this T o, object x) => !o.Is(x);
		public static bool IsNot<T>(this T? o, T x) where T : struct => !o.Is(x);
		public static bool IsNot<T>(this T o, T? x) where T : struct => !o.Is(x);
	}
}