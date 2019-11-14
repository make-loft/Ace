using System;

namespace Ace
{
	public static partial class TypeOf<T>
	{
		public static T Match() =>
			default;
		public static T Match<A>(Func<A> a) =>
			TypeOf<A>.Raw.Is(Raw) ? a().To<T>() : Match();
		public static T Match<A, B>(Func<A> a, Func<B> b) =>
			TypeOf<A>.Raw.Is(Raw) ? a().To<T>() : Match(b);
		public static T Match<A, B, C>(Func<A> a, Func<B> b, Func<C> c) =>
			TypeOf<A>.Raw.Is(Raw) ? a().To<T>() : Match(b, c);
		public static T Match<A, B, C, D>(Func<A> a, Func<B> b, Func<C> c, Func<D> d) =>
			TypeOf<A>.Raw.Is(Raw) ? a().To<T>() : Match(b, c, d);
		public static T Match<A, B, C, D, E>(Func<A> a, Func<B> b, Func<C> c, Func<D> d, Func<E> e) =>
			TypeOf<A>.Raw.Is(Raw) ? a().To<T>() : Match(b, c, d, e);
		public static T Match<A, B, C, D, E, F>(Func<A> a, Func<B> b, Func<C> c, Func<D> d, Func<E> e, Func<F> f) =>
			TypeOf<A>.Raw.Is(Raw) ? a().To<T>() : Match(b, c, d, e, f);
	}
}
