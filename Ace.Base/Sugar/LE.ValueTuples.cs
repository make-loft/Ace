#if ValueTuples
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ace;

namespace Ace.Base.Console
{
	public static partial class LE
	{
		public static bool IsNot<T>(this T x, params object[] y) where T : ITuple => !x.Is(y);

		public static bool Is<T>(this T x, params object[] y) where T : ITuple
		{
			if (x.Length.IsNot(y.Length)) return false;
			for (var i = 0; i < y.Length; i++)
			{
				if (x[i].IsNot(y[i])) return false;
			}

			return true;
		}

		//public static bool Is<A>(this (A) x, A a) => x.Is((a));
		public static bool Is<A, B>(this (A, B) x, A a, B b) => x.Is((a, b));
		public static bool Is<A, B, C>(this (A, B, C) x, A a, B b, C c) => x.Is((a, b, c));
		public static bool Is<A, B, C, D>(this (A, B, C, D) x, A a, B b, C c, D d) => x.Is((a, b, c, d));
		// etc...


		//public static bool Is<A>(this (A) x, A a) => !x.Is((a));
		public static bool IsNot<A, B>(this (A, B) x, A a, B b) => !x.Is((a, b));
		public static bool IsNot<A, B, C>(this (A, B, C) x, A a, B b, C c) => !x.Is((a, b, c));
		public static bool IsNot<A, B, C, D>(this (A, B, C, D) x, A a, B b, C c, D d) => !x.Is((a, b, c, d));
		// etc...

		public static bool IsAny<TTuple>(this object o, TTuple tuple) where TTuple : ITuple => tuple.Contains(o);

		//public static bool IsAny<A>(this object o, A a) => o.Is(a); 
		public static bool IsAny<A, B>(this object o, A a, B b) => o.Is(a) || o.Is(b);
		public static bool IsAny<A, B, C>(this object o, A a, B b, C c) => o.IsAny(a, b) || o.Is(c);
		public static bool IsAny<A, B, C, D>(this object o, A a, B b, C c, D d) => o.IsAny(a, b, c) || o.Is(d);
		public static bool IsAny<A, B, C, D, E>(this object o, A a, B b, C c, D d, E e) => o.IsAny(a, b, c, d) || o.Is(e);
		public static bool IsAny<A, B, C, D, E, F>(this object o, A a, B b, C c, D d, E e , F f) => o.IsAny(a, b, c, d, e) || o.Is(f);
		public static bool IsAny<A, B, C, D, E, F, G>(this object o, A a, B b, C c, D d, E e , F f, G g) => o.IsAny(a, b, c, d, e, f) || o.Is(g);
		public static bool IsAny<A, B, C, D, E, F, G, H>(this object o, A a, B b, C c, D d, E e , F f, G g, H h) => o.IsAny(a, b, c, d, e, f, g) || o.Is(h);

		public static bool Contains<TTuple>(this TTuple tuple, object o) where TTuple : ITuple
		{
			for (var i = 0; i < tuple.Length; i++)
			{
				if (tuple[i].Is(o)) return true;
			}

			return false;
		}
	}
}
#endif