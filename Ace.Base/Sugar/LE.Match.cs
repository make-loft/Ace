using System;
// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable InconsistentNaming
    public static partial class LE
    {
		public static TOut Match<TIn, TOut>(this TIn context, Func<TOut> nullCase = null)
			=> context.InvokeMatcher((Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null,
				(Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A>(this TIn context,
			Func<A, TOut> a, Func<TOut> nullCase = null)
			where A : TIn
			=> context.InvokeMatcher(a, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null,
				(Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<TOut> nullCase = null)
			where A : TIn where B : TIn
			=> context.InvokeMatcher(a, b, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null,
				(Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn
			=> context.InvokeMatcher(a, b, c, (Func<TIn, TOut>) null,
				(Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C, D>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn
			=> context.InvokeMatcher(a, b, c, d,
				(Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C, D, E>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d,
			Func<E, TOut> e, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn
			=> context.InvokeMatcher(a, b, c, d, e, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C, D, E, F>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d,
			Func<E, TOut> e, Func<F, TOut> f, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn
			=> context.InvokeMatcher(a, b, c, d, e, f, (Func<TIn, TOut>) null, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C, D, E, F, G>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d,
			Func<E, TOut> e, Func<F, TOut> f, Func<G, TOut> g, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn where G : TIn
			=> context.InvokeMatcher(a, b, c, d, e, f, g, (Func<TIn, TOut>) null, nullCase);

		public static TOut Match<TIn, TOut, A, B, C, D, E, F, G, H>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d,
			Func<E, TOut> e, Func<F, TOut> f, Func<G, TOut> g, Func<H, TOut> h, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn where G : TIn where H : TIn
			=> context.InvokeMatcher(a, b, c, d, e, f, g, h, nullCase);
		
		private static TOut InvokeMatcher<TIn, TOut, A, B, C, D, E, F, G, H>(this TIn context,
			Func<A, TOut> a, Func<B, TOut> b, Func<C, TOut> c, Func<D, TOut> d,
			Func<E, TOut> e, Func<F, TOut> f, Func<G, TOut> g, Func<H, TOut> h, Func<TOut> nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn where G : TIn where H : TIn
		{
			switch (context)
			{
				case A aa when a != null: return a.Invoke(aa);
				case B bb when b != null: return b.Invoke(bb);
				case C cc when c != null: return c.Invoke(cc);
				case D dd when d != null: return d.Invoke(dd);
				case E ee when e != null: return e.Invoke(ee);
				case F ff when f != null: return f.Invoke(ff);
				case G gg when g != null: return g.Invoke(gg);
				case H hh when h != null: return h.Invoke(hh);
				default:
					return nullCase != null && context.IsNull()
						? nullCase.Invoke()
						: throw new ArgumentException($"Undefined case for '{context}'");
			}
		}
    }
}