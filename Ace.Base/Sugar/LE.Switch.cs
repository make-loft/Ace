using System;
// ReSharper disable once CheckNamespace
namespace Ace.Base.Sugar
{
	// ReSharper disable InconsistentNaming
	public static partial class LE
	{
		public static void Switch<TIn>(this TIn context, Action nullCase = null)
			=> context.InvokeSwitcher((Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null,
				(Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, nullCase);

		public static void Switch<TIn, A>(this TIn context,
			Action<A> a, Action nullCase = null)
			where A : TIn
			=> context.InvokeSwitcher(a, (Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null,
				(Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, nullCase);

		public static void Switch<TIn, A, B>(this TIn context,
			Action<A> a, Action<B> b, Action nullCase = null)
			where A : TIn where B : TIn
			=> context.InvokeSwitcher(a, b, (Action<TIn>)null, (Action<TIn>)null,
				(Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, nullCase);

		public static void Switch<TIn, A, B, C>(this TIn context,
			Action<A> a, Action<B> b, Action<C> c, Action nullCase = null)
			where A : TIn where B : TIn where C : TIn
			=> context.InvokeSwitcher(a, b, c, (Action<TIn>)null,
				(Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, nullCase);

		public static void Switch<TIn, A, B, C, D>(this TIn context,
			Action<A> a, Action<B> b, Action<C> c, Action<D> d, Action nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn
			=> context.InvokeSwitcher(a, b, c, d,
				(Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, nullCase);

		public static void Switch<TIn, A, B, C, D, E>(this TIn context,
			Action<A> a, Action<B> b, Action<C> c, Action<D> d,
			Action<E> e, Action nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn
			=> context.InvokeSwitcher(a, b, c, d, e, (Action<TIn>)null, (Action<TIn>)null, (Action<TIn>)null, nullCase);

		public static void Switch<TIn, A, B, C, D, E, F>(this TIn context,
			Action<A> a, Action<B> b, Action<C> c, Action<D> d,
			Action<E> e, Action<F> f, Action nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn
			=> context.InvokeSwitcher(a, b, c, d, e, f, (Action<TIn>)null, (Action<TIn>)null, nullCase);

		public static void Switch<TIn, A, B, C, D, E, F, G>(this TIn context,
			Action<A> a, Action<B> b, Action<C> c, Action<D> d,
			Action<E> e, Action<F> f, Action<G> g, Action nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn where G : TIn
			=> context.InvokeSwitcher(a, b, c, d, e, f, g, (Action<TIn>)null, nullCase);

		public static void Switch<TIn, A, B, C, D, E, F, G, H>(this TIn context,
			Action<A> a, Action<B> b, Action<C> c, Action<D> d,
			Action<E> e, Action<F> f, Action<G> g, Action<H> h, Action nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn where G : TIn where H : TIn
			=> context.InvokeSwitcher(a, b, c, d, e, f, g, h, nullCase);

		private static void InvokeSwitcher<TIn, A, B, C, D, E, F, G, H>(this TIn context,
			Action<A> a, Action<B> b, Action<C> c, Action<D> d,
			Action<E> e, Action<F> f, Action<G> g, Action<H> h, Action nullCase = null)
			where A : TIn where B : TIn where C : TIn where D : TIn where E : TIn where F : TIn where G : TIn where H : TIn
		{
			switch (context)
			{
				case A aa when a.Is(): a.Invoke(aa); break;
				case B bb when b.Is(): b.Invoke(bb); break;
				case C cc when c.Is(): c.Invoke(cc); break;
				case D dd when d.Is(): d.Invoke(dd); break;
				case E ee when e.Is(): e.Invoke(ee); break;
				case F ff when f.Is(): f.Invoke(ff); break;
				case G gg when g.Is(): g.Invoke(gg); break;
				case H hh when h.Is(): h.Invoke(hh); break;
				default:
					if (nullCase.Is() && context.IsNot())
						nullCase.Invoke();
					else throw new ArgumentException($"Undefined case for '{context}'");
					break;
			}
		}
	}
}
