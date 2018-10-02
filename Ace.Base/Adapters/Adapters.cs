using System;
using System.Linq;
using Ace;

#if !NETSTANDARD
// ReSharper disable once CheckNamespace
namespace System.Reflection
{
	internal static class Reflection
	{
		public static T GetCustomAttribute<T>(this Type type) where T: class =>
			type.GetCustomAttributes(TypeOf<T>.Raw, true).FirstOrDefault() as T;

		public static T GetCustomAttribute<T>(this MemberInfo member) where T : class =>
			member?.GetCustomAttributes(TypeOf<T>.Raw, true).FirstOrDefault() as T;
	}
}
#endif

namespace System.TypeInfoAdapter
{
	public static class TypeInfo
	{
		public static Type GetTypeInfo(this Type type) => type;
	}
}

namespace Ace.Adapters
{
	[Flags]
	public enum BindingFlags
	{
		DeclaredOnly = 2,
		ExactBinding = 65536,
		FlattenHierarchy = 64,
		IgnoreCase = 1,
		Instance = 4,
		NonPublic = 32,
		OptionalParamBinding = 262144,
		Public = 16,
		Static = 8,
	}
}

namespace System
{
	public delegate void Action<T1, T2, T3, T4, T5>(T1 a, T2 b, T3 c, T4 d, T5 e);
	public delegate void Action<T1, T2, T3, T4, T5, T6>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f);
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g);
	public delegate void Action<T1, T2, T3, T4, T5, T6, T7, T8>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h);
	
	public delegate TResult Func<T1, T2, T3, T4, T5, TResult>(T1 a, T2 b, T3 c, T4 d, T5 e);
	public delegate TResult Func<T1, T2, T3, T4, T5, T6, TResult>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f);
	public delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, TResult>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g);
	public delegate TResult Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(T1 a, T2 b, T3 c, T4 d, T5 e, T6 f, T7 g, T8 h);
}