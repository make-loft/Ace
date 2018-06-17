using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Ace
{
	public static class TypeOf
	{
		public static TypeData ToRipeData(this Type type) => new TypeData(type);

		public static readonly TypeData Object = typeof(object).ToRipeData();
		public static readonly TypeData String = typeof(string).ToRipeData();
		public static readonly TypeData Array = typeof(Array).ToRipeData();
		public static readonly TypeData Type = typeof(Type).ToRipeData();
		public static readonly TypeData List = typeof(List<>).ToRipeData();
		public static readonly TypeData IList = typeof(IList<>).ToRipeData();
		public static readonly TypeData Dictionary = typeof(Dictionary<,>).ToRipeData();
		public static readonly TypeData IDictionary = typeof(IDictionary<,>).ToRipeData();
		public static readonly TypeData KeyValuePair = typeof(KeyValuePair<,>).ToRipeData();
		public static readonly TypeData DictionaryEntry = typeof(DictionaryEntry).ToRipeData();

		public static Assembly SystemAssembly => Object.Assembly;

		//static TypeOf() { } // see: https://github.com/dotnet/coreclr/issues/17981
	}

	// ReSharper disable StaticMemberInGenericType
	public struct TypeOf<T>
	{
		public static readonly Type Raw = typeof(T);
		private static readonly TypeData Data = Raw.ToRipeData();

		public static Assembly Assembly => Data.Assembly;
		public static string AssemblyQualifiedName => Data.AssemblyQualifiedName;
		public static string FullName => Data.FullName;
		public static string Name => Data.Name;
		public static string Namespace => Data.Namespace;

		public static bool HasElementType => Data.HasElementType;
		public static bool IsArray => Data.IsArray;
		public static bool IsAbstract => Data.IsAbstract;
		public static bool IsByRef => Data.IsByRef;
		public static bool IsClass => Data.IsClass;
		public static bool IsEnum => Data.IsEnum;
		public static bool IsGenericParameter => Data.IsGenericParameter;
		public static bool IsGenericType => Data.IsGenericType;
		public static bool IsGenericTypeDefinition => Data.IsGenericTypeDefinition;
		public static bool IsInterface => Data.IsInterface;
		public static bool IsNested => Data.IsNested;
		public static bool IsNestedAssembly => Data.IsNestedAssembly;
		public static bool IsNestedFamANDAssem => Data.IsNestedFamANDAssem;
		public static bool IsNestedFamily => Data.IsNestedFamily;
		public static bool IsNestedFamORAssem => Data.IsNestedFamORAssem;
		public static bool IsNestedPrivate => Data.IsNestedPrivate;
		public static bool IsNestedPublic => Data.IsNestedPublic;
		public static bool IsNotPublic => Data.IsNotPublic;
		public static bool IsPointer => Data.IsPointer;
		public static bool IsPrimitive => Data.IsPrimitive;
		public static bool IsPublic => Data.IsPublic;
		public static bool IsSealed => Data.IsSealed;
		public static bool IsSpecialName => Data.IsSpecialName;
		public static bool IsValueType => Data.IsValueType;
		public static bool IsVisible => Data.IsVisible;

		//static TypeOf() { } // see: https://github.com/dotnet/coreclr/issues/17981
	}
}