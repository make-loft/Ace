using System;
// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable InconsistentNaming
	public static partial class New
	{
		public static Tuple<A, B, C, D, E, F, G, H> Tuple<A, B, C, D, E, F, G, H>(
			A a, B b, C c, D d, E e, F f, G g, H h) => new Tuple<A, B, C, D, E, F, G, H>(a, b, c, d, e, f, g, h);

		public static Tuple<A, B, C, D, E, F, G> Tuple<A, B, C, D, E, F, G>(
			A a, B b, C c, D d, E e, F f, G g) => new Tuple<A, B, C, D, E, F, G>(a, b, c, d, e, f, g);

		public static Tuple<A, B, C, D, E, F> Tuple<A, B, C, D, E, F>(
			A a, B b, C c, D d, E e, F f) => new Tuple<A, B, C, D, E, F>(a, b, c, d, e, f);

		public static Tuple<A, B, C, D, E> Tuple<A, B, C, D, E>(
			A a, B b, C c, D d, E e) => new Tuple<A, B, C, D, E>(a, b, c, d, e);
		
		public static Tuple<A, B, C, D> Tuple<A, B, C, D>(
			A a, B b, C c, D d) => new Tuple<A, B, C, D>(a, b, c, d);
		
		public static Tuple<A, B, C> Tuple<A, B, C>(
			A a, B b, C c) => new Tuple<A, B, C>(a, b, c);
		
		public static Tuple<A, B> Tuple<A, B>(
			A a, B b) => new Tuple<A, B>(a, b);
		
		public static Tuple<A> Tuple<A>(
			A a) => new Tuple<A>(a);
	}
}