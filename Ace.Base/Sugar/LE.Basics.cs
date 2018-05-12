using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.TypeInfoAdapter;

// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable once InconsistentNaming
	/* LanguageExtensions */
	public static partial class LE
	{
		/* a hack to define legacy runtimes todo with C# 7.3 */
		private static readonly bool IsLegacyRuntime =
			typeof(Environment).GetProperties()[0].Name != "CommandLine";

		public static TX Put<T, TX>(this T o, TX x) => x;
		public static TX Put<T, TX>(this T o, ref TX x) => x;

		public static object ChangeType<T>(this object o) =>
			o == null || TypeOf<T>.IsValueType || o is IConvertible ? Convert.ChangeType(o, typeof(T), null) : o;

		public static T To<T>(this T o) => o;
		public static T To<T>(this T o, out T x) => x = o;
		public static T To<T>(this object o) => (T) o.ChangeType<T>();
		public static T To<T>(this object o, out T x) => x = (T) o.ChangeType<T>();
		public static T? To<T>(this object o, out T? x) where T : struct => x = (T?) o;

		public static T As<T>(this T o) => o;
		public static T As<T>(this T o, out T x) => x = o;
		public static T As<T>(this object o, T fallbackValue = default(T)) => o.Is<T>() ? (T) o : fallbackValue;
		public static T As<T>(this object o, out T x, T fallbackValue = default(T)) => x = o.As(fallbackValue);

		public static bool Is<T>(this T o) => (IsLegacyRuntime && TypeOf<T>.IsValueType) || o != null;
		public static bool Is<T>(this T o, out T x) => (x = o).Is();
		public static bool Is<T>(this object o) => o is T; // o != null && typeof(T).IsAssignableFrom(o.GetType());	
		public static bool Is<T>(this object o, out T x, T fallbackValue = default(T)) =>
			(x = o.Is<T>().To(out var b) ? (T) o : fallbackValue).Put(b);

		public static bool IsNull(this object o) => o is null;
		public static bool IsNull<T>(this T o) => (IsLegacyRuntime && !TypeOf<T>.IsValueType) && o == null;
		public static bool IsNull<T>(this T o, out T x) => (x = o).IsNull();
		public static bool IsNull<T>(this object o, out T x, T fallbackValue = default(T)) =>
			(x = o.IsNull().To(out var b) ? (T) o : fallbackValue).Put(b);

		public static bool Equals<T>(T a, T b) => EqualityComparer<T>.Default.Equals(a, b); /* boxing free equals */

		public static bool Is<T>(this T o, T x) => Equals(o, x); /* Equals<T>(o, x); */
		public static bool Is<T>(this T o, object x) => x.Is<T>() && Equals(o, (T) x);
		public static bool Is<T>(this object o, T x) => o.Is<T>() && Equals((T) o, x);
		public static bool Is(this object o, object x) => Equals(o, x);
		
		public static bool IsNot<T>(this T o, T x) => !o.Is(x);
		public static bool IsNot<T>(this object o, T x) => !o.Is(x);
		public static bool IsNot<T>(this T o, object x) => !o.Is(x);
		public static bool IsNot(this object o, object x) => !o.Is(x);
		
		public static bool Is<T>(this T? o) where T : struct => o.HasValue; // o is T
		public static bool Is<T>(this T? o, T x) where T : struct => o.HasValue && Equals(o.Value, x);
		public static bool Is<T>(this T o, T? x) where T : struct => x.HasValue && Equals(o, x.Value);
		public static bool IsNot<T>(this T? o, T x) where T : struct => !o.Is(x);
		public static bool IsNot<T>(this T o, T? x) where T : struct => !o.Is(x);
		public static bool IsNull<T>(this T? o) where T : struct => !o.HasValue; /* is null */
	}
}