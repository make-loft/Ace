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

#if WINDOWS_PHONE
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

namespace Ace
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum, Inherited = false)]
	public class DataContractAttribute : Attribute
	{
		public bool IsNameSetExplicitly => Name.Is();
		public bool IsNamespaceSetExplicitly => Namespace.Is();

		public bool IsReference { get; set; }
		public string Namespace { get; set; }
		public string Name { get; set; }
	}

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	public class CollectionDataContractAttribute : DataContractAttribute
	{
		public bool IsItemNameSetExplicitly => ItemName.Is();
		public bool IsKeyNameSetExplicitly => KeyName.Is();
		public bool IsValueNameSetExplicitly => ValueName.Is();

		public bool IsReferenceSetExplicitly { get; set; }
		public string ItemName { get; set; }
		public string KeyName { get; set; }
		public string ValueName { get; set; }
	}

	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false)]
	public class DataMemberAttribute : Attribute
	{
		public string Name { get; set; }
		public bool IsNameSetExplicitly => Name.Is();
		public int Order { get; set; } = -1;
		public bool IsRequired { get; set; }
		public bool EmitDefaultValue { get; set; }
	}
	
	[AttributeUsage(AttributeTargets.Method, Inherited=false)]
	public sealed class OnSerializingAttribute : Attribute 
	{
	}
 
	[AttributeUsage(AttributeTargets.Method, Inherited=false)]
	public sealed class OnSerializedAttribute : Attribute 
	{
	}
 
	[AttributeUsage(AttributeTargets.Method, Inherited=false)]
	public sealed class OnDeserializingAttribute : Attribute 
	{
	}
 
	[AttributeUsage(AttributeTargets.Method, Inherited=false)]
	public sealed class OnDeserializedAttribute : Attribute 
	{
	}

	public struct StreamingContext
	{
		public StreamingContext(StreamingContextStates state, object additional = null)
		{
			State = state;
			Context = additional;
		}

		public object Context { get; internal set; }

		public override bool Equals(object obj) => obj is StreamingContext context &&
		                                           context.Context == Context &&
		                                           context.State == State;

		public override int GetHashCode() => (int) State;

		public StreamingContextStates State { get; internal set; }
	}

	// **********************************************************
	// Keep these in sync with the version in vm\runtimehandles.h
	// **********************************************************
	[Flags]
	public enum StreamingContextStates {
		CrossProcess=0x01,
		CrossMachine=0x02,
		File        =0x04,
		Persistence =0x08,
		Remoting    =0x10,
		Other       =0x20,
		Clone       =0x40,
		CrossAppDomain =0x80,
		All         =0xFF,
	}
}