using System;

// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable InconsistentNaming
	public static partial class LE
	{
		public static T Call<T>(this T o, Action action)
		{
			action();
			return o;
		}

		public static TR Call<T, TR>(this T o, Func<T, TR> func) => func(o);

		public static T Use<T>(this T o, Action<T> action)
		{
			action(o);
			return o;
		}

		public static T Use<T, TR>(this T o, Func<TR> func) => func().Put(o);
		public static T Uses<T, TR>(this T o, Func<T, TR> func) => func(o).Put(o);
		public static T Use<T, A>(this T o, out A x, A a = default) => (x = a).Put(o); // o.Use(out var x, 2)...
	}
}