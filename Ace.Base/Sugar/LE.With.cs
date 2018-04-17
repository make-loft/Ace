// ReSharper disable once CheckNamespace
namespace Ace
{
	// ReSharper disable InconsistentNaming
	public static partial class LE
	{
		public static T With<T>(this T o, params object[] pattern) => o;
		
		public static T With<T>(this T o) => o;
		public static T With<T, A>(this T o, A a) => o;
		public static T With<T, A, B>(this T o, A a, B b) => o;
		public static T With<T, A, B, C>(this T o, A a, B b, C c) => o;
		public static T With<T, A, B, C, D>(this T o, A a, B b, C c, D d) => o;
		public static T With<T, A, B, C, D, E>(this T o, A a, B b, C c, D d, E e) => o;
		public static T With<T, A, B, C, D, E, F>(this T o, A a, B b, C c, D d, E e, F f) => o;
		public static T With<T, A, B, C, D, E, F, G>(this T o, A a, B b, C c, D d, E e, F f, G g) => o;
		public static T With<T, A, B, C, D, E, F, G, H>(this T o, A a, B b, C c, D d, E e, F f, G g, H h) => o;
		
		public static T With<T, A>(this T o, out A x, A a = default(A)) => (x = a).Put(o);
		public static A With<A>(out A x, A a = default(A)) => x = a;
		public static object With() => CN.Null.With();
		
		public delegate void WithAction();
		public delegate void WithAction<in T>(T o);

		public static T With<T>(this T o, WithAction action)
		{
			action();
			return o;
		}
		
		public static T With<T>(this T o, WithAction<T> action)
		{
			action(o);
			return o;
		}
	}
}