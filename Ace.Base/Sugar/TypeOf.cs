using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Ace.Sugar;

// ReSharper disable once CheckNamespace
namespace Ace
{
	public static class TypeOf
	{
		private static readonly object SyncRoot = new object();
		
		private static readonly Dictionary<Type, RipeType> RawToRipe = new Dictionary<Type, RipeType>();

		public static RipeType ToRipeType(this Type raw) =>
			RawToRipe.TryGetValue(raw, out var ripe)
				? ripe
				: Lock.Invoke(SyncRoot, () => RawToRipe.TryGetValue(raw, out ripe)
					? ripe // may catch an item created into a different thread
					: RawToRipe[raw] = new RipeType(raw));

		public static RipeType GetRipeType(this object o) => o.GetType().ToRipeType();

		public static readonly RipeType Object = typeof(object).ToRipeType();
		public static readonly RipeType String = typeof(string).ToRipeType();
		public static readonly RipeType Array = typeof(Array).ToRipeType();
		public static readonly RipeType Type = typeof(Type).ToRipeType();
		public static readonly RipeType List = typeof(List<>).ToRipeType();
		public static readonly RipeType IList = typeof(IList<>).ToRipeType();
		public static readonly RipeType Dictionary = typeof(Dictionary<,>).ToRipeType();
		public static readonly RipeType IDictionary = typeof(IDictionary<,>).ToRipeType();
		public static readonly RipeType KeyValuePair = typeof(KeyValuePair<,>).ToRipeType();
		public static readonly RipeType DictionaryEntry = typeof(DictionaryEntry).ToRipeType();

		public static Assembly SystemAssembly => Object.Assembly;
		public static bool IsSystemType(this Type type) => type.Assembly.Is(SystemAssembly);
		public static bool IsSystemType(this RipeType type) => type.Assembly.Is(SystemAssembly);

		//static TypeOf() { } // see: https://github.com/dotnet/coreclr/issues/17981
	}

	// ReSharper disable StaticMemberInGenericType
	public static class TypeOf<T>
	{
		/* Important! Should be a readonly variable for best performance */ 
		private static readonly RipeType Ripe = typeof(T).ToRipeType();
		public static readonly Type Raw = Ripe.Raw;

		public static readonly Assembly Assembly = Ripe.Assembly;
		public static string AssemblyQualifiedName => Ripe.AssemblyQualifiedName;
		public static string FullName => Ripe.FullName;
		public static readonly string Name = Ripe.Name;
		public static string Namespace => Ripe.Namespace;

		public static bool HasElementType => Ripe.HasElementType;
		public static bool IsArray => Ripe.IsArray;
		public static bool IsAbstract => Ripe.IsAbstract;
		public static bool IsByRef => Ripe.IsByRef;
		public static bool IsClass => Ripe.IsClass;
		public static bool IsEnum => Ripe.IsEnum;
		public static bool IsGenericParameter => Ripe.IsGenericParameter;
		public static bool IsGenericType => Ripe.IsGenericType;
		public static bool IsGenericTypeDefinition => Ripe.IsGenericTypeDefinition;
		public static bool IsInterface => Ripe.IsInterface;
		public static bool IsNested => Ripe.IsNested;
		public static bool IsNestedAssembly => Ripe.IsNestedAssembly;
		public static bool IsNestedFamANDAssem => Ripe.IsNestedFamANDAssem;
		public static bool IsNestedFamily => Ripe.IsNestedFamily;
		public static bool IsNestedFamORAssem => Ripe.IsNestedFamORAssem;
		public static bool IsNestedPrivate => Ripe.IsNestedPrivate;
		public static bool IsNestedPublic => Ripe.IsNestedPublic;
		public static bool IsNotPublic => Ripe.IsNotPublic;
		public static bool IsPointer => Ripe.IsPointer;
		public static bool IsPrimitive => Ripe.IsPrimitive;
		public static bool IsPublic => Ripe.IsPublic;
		public static bool IsSealed => Ripe.IsSealed;
		public static bool IsSpecialName => Ripe.IsSpecialName;
		public static readonly bool IsValueType = Ripe.IsValueType;
		public static bool IsVisible => Ripe.IsVisible;

		//static TypeOf() { } // see: https://github.com/dotnet/coreclr/issues/17981
	}
}