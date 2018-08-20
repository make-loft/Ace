using System;
// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable InconsistentNaming
	public static partial class LE
	{
		public static bool[] Check<A, B, C, D, E, F, G, H>(this Tuple<A, B, C, D, E, F, G, H> t,
			A a, B b, C c, D d, E e, F f, G g, H h) => t.Check(
			t.Item1.Is(a), t.Item2.Is(b), t.Item3.Is(c), t.Item4.Is(d), 
			t.Item5.Is(e), t.Item6.Is(f), t.Item7.Is(g), t.Rest.Is(h));
		
		public static bool[] Check<A, B, C, D, E, F, G>(this Tuple<A, B, C, D, E, F, G> t,
			A a, B b, C c, D d, E e, F f, G g) => t.Check(
			t.Item1.Is(a), t.Item2.Is(b), t.Item3.Is(c), t.Item4.Is(d), 
			t.Item5.Is(e), t.Item6.Is(f), t.Item7.Is(g));
		
		public static bool[] Check<A, B, C, D, E, F>(this Tuple<A, B, C, D, E, F> t,
			A a, B b, C c, D d, E e, F f) => t.Check(
			t.Item1.Is(a), t.Item2.Is(b), t.Item3.Is(c), t.Item4.Is(d), 
			t.Item5.Is(e), t.Item6.Is(f));
		
		public static bool[] Check<A, B, C, D, E>(this Tuple<A, B, C, D, E> t,
			A a, B b, C c, D d, E e) => t.Check(
			t.Item1.Is(a), t.Item2.Is(b), t.Item3.Is(c), t.Item4.Is(d), 
			t.Item5.Is(e));
		
		public static bool[] Check<A, B, C, D>(this Tuple<A, B, C, D> t,
			A a, B b, C c, D d) => t.Check(
			t.Item1.Is(a), t.Item2.Is(b), t.Item3.Is(c), t.Item4.Is(d));
		
		public static bool[] Check<A, B, C>(this Tuple<A, B, C> t,
			A a, B b, C c) => t.Check(
			t.Item1.Is(a), t.Item2.Is(b), t.Item3.Is(c));
		
		public static bool[] Check<A, B>(this Tuple<A, B> t,
			A a, B b) => t.Check(
			t.Item1.Is(a), t.Item2.Is(b));
		
		public static bool[] Check<A>(this Tuple<A> t,
			A a) => t.Check(
			t.Item1.Is(a));

		public static Tuple<A, B, C, D, E, F, G, H> ToTuple<T, A, B, C, D, E, F, G, H>(this T o,
			A a, B b, C c, D d, E e, F f, G g, H h) => New.Tuple(a, b, c, d, e, f, g, h);
		
		public static Tuple<A, B, C, D, E, F, G> ToTuple<T, A, B, C, D, E, F, G>(this T o,
			A a, B b, C c, D d, E e, F f, G g) => New.Tuple(a, b, c, d, e, f, g);
		
		public static Tuple<A, B, C, D, E, F> ToTuple<T, A, B, C, D, E, F>(this T o,
			A a, B b, C c, D d, E e, F f) => New.Tuple(a, b, c, d, e, f);
		
		public static Tuple<A, B, C, D, E> ToTuple<T, A, B, C, D, E>(this T o,
			A a, B b, C c, D d, E e) => New.Tuple(a, b, c, d, e);
		
		public static Tuple<A, B, C, D> ToTuple<T, A, B, C, D>(this T o,
			A a, B b, C c, D d) => New.Tuple(a, b, c, d);
		
		public static Tuple<A, B, C> ToTuple<T, A, B, C>(this T o,
			A a, B b, C c) => New.Tuple(a, b, c);
		
		public static Tuple<A, B> ToTuple<T, A, B>(this T o,
			A a, B b) => New.Tuple(a, b);
		
		public static Tuple<A> ToTuple<T, A>(this T o,
			A a) => New.Tuple(a);
		
		public static Tuple<T> ToTuple<T>(this T o) => New.Tuple(o);

		public static Tuple<A, B, C, D, E, F, G, H> To<A, B, C, D, E, F, G, H>(this Tuple<A, B, C, D, E, F, G, H> t,
			out A a, out B b, out C c, out D d, out E e, out F f, out G g, out H h) => t.With(
			a = t.Item1, b = t.Item2, c = t.Item3, d = t.Item4, e = t.Item5, f = t.Item6, g = t.Item7, h = t.Rest);
		
		public static Tuple<A, B, C, D, E, F, G> To<A, B, C, D, E, F, G>(this Tuple<A, B, C, D, E, F, G> t,
			out A a, out B b, out C c, out D d, out E e, out F f, out G g) => t.With(
			a = t.Item1, b = t.Item2, c = t.Item3, d = t.Item4, e = t.Item5, f = t.Item6, g = t.Item7);
		
		public static Tuple<A, B, C, D, E, F> To<A, B, C, D, E, F>(this Tuple<A, B, C, D, E, F> t,
			out A a, out B b, out C c, out D d, out E e, out F f) => t.With(
			a = t.Item1, b = t.Item2, c = t.Item3, d = t.Item4, e = t.Item5, f = t.Item6);
		
		public static Tuple<A, B, C, D, E> To<A, B, C, D, E>(this Tuple<A, B, C, D, E> t,
			out A a, out B b, out C c, out D d, out E e) => t.With(
			a = t.Item1, b = t.Item2, c = t.Item3, d = t.Item4, e = t.Item5);
		
		public static Tuple<A, B, C, D> To<A, B, C, D>(this Tuple<A, B, C, D> t,
			out A a, out B b, out C c, out D d) => t.With(
			a = t.Item1, b = t.Item2, c = t.Item3, d = t.Item4);
		
		public static Tuple<A, B, C> To<A, B, C>(this Tuple<A, B, C> t,
			out A a, out B b, out C c) => t.With(
			a = t.Item1, b = t.Item2, c = t.Item3);
		
		public static Tuple<A, B> To<A, B>(this Tuple<A, B> t,
			out A a, out B b) => t.With(
			a = t.Item1, b = t.Item2);
		
		public static Tuple<A> To<A>(this Tuple<A> t,
			out A a) => t.With(
			a = t.Item1);
	}
}