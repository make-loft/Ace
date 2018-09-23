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

#if SINCE_CSHARP_7_3
		public static T With<T>(this ref T o, params object[] pattern) where T : struct => o;

		public static T With<T>(this ref T o) where T : struct => o;
		public static T With<T, A>(this ref T o, A a) where T : struct => o;
		public static T With<T, A, B>(this ref T o, A a, B b) where T : struct => o;
		public static T With<T, A, B, C>(this ref T o, A a, B b, C c) where T : struct => o;
		public static T With<T, A, B, C, D>(this ref T o, A a, B b, C c, D d) where T : struct => o;
		public static T With<T, A, B, C, D, E>(this ref T o, A a, B b, C c, D d, E e) where T : struct => o;
		public static T With<T, A, B, C, D, E, F>(this ref T o, A a, B b, C c, D d, E e, F f) where T : struct => o;
		public static T With<T, A, B, C, D, E, F, G>(this ref T o, A a, B b, C c, D d, E e, F f, G g) where T : struct => o;
		public static T With<T, A, B, C, D, E, F, G, H>(this ref T o, A a, B b, C c, D d, E e, F f, G g, H h) where T : struct => o;
#endif
	}
}