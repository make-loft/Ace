using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Ace
{
	// performance optimized
	public static class TypeOf
	{
		public static readonly Type Object = typeof(object);
		public static readonly Type String = typeof(string);
		public static readonly Type Type = typeof(Type);
		public static readonly Type List = typeof(List<>);
		public static readonly Type IList = typeof(IList<>);
		public static readonly Type Dictionary = typeof(Dictionary<,>);
		public static readonly Type IDictionary = typeof(IDictionary<,>);
		public static readonly Type KeyValuePair = typeof(KeyValuePair<,>);
		public static readonly Type DictionaryEntry = typeof(DictionaryEntry);

		public static readonly Assembly SystemAssembly = Object.Assembly;

#if CoreCLR
		static TypeOf() { } // see: https://github.com/dotnet/coreclr/issues/17981
#endif
	}

	// ReSharper disable StaticMemberInGenericType
	public static class TypeOf<T>
	{
		public static readonly Type Info = typeof(T);

		private static string _name, _fullName, _assemblyQualifiedName;
		private static Assembly _assembly;

		public static string Name { get; } = _name ?? Info.Name.To(out _name);
		public static string FullName { get; } = _fullName ?? Info.FullName.To(out _fullName);
		public static Assembly Assembly { get; } = _assembly ?? Info.Assembly.To(out _assembly);
		public static string AssemblyQualifiedName { get; } = _assemblyQualifiedName ?? Info.AssemblyQualifiedName.To(out _assemblyQualifiedName);

		private static readonly Lazy<bool> HasElementTypeLazy = new Lazy<bool>(() => Info.HasElementType);
		private static readonly Lazy<bool> IsArrayLazy = new Lazy<bool>(() => Info.IsArray);
		private static readonly Lazy<bool> IsAbstractLazy = new Lazy<bool>(() => Info.IsAbstract);
		private static readonly Lazy<bool> IsByRefLazy = new Lazy<bool>(() => Info.IsByRef);
		private static readonly Lazy<bool> IsClassLazy = new Lazy<bool>(() => Info.IsClass);
		private static readonly Lazy<bool> IsEnumLazy = new Lazy<bool>(() => Info.IsEnum);
		private static readonly Lazy<bool> IsGenericParameterLazy = new Lazy<bool>(() => Info.IsGenericParameter);
		private static readonly Lazy<bool> IsGenericTypeLazy = new Lazy<bool>(() => Info.IsGenericType);
		private static readonly Lazy<bool> IsGenericTypeDefinitionLazy = new Lazy<bool>(() => Info.IsGenericTypeDefinition);
		private static readonly Lazy<bool> IsInterfaceLazy = new Lazy<bool>(() => Info.IsInterface);
		private static readonly Lazy<bool> IsNestedLazy = new Lazy<bool>(() => Info.IsNested);
		private static readonly Lazy<bool> IsNestedAssemblyLazy = new Lazy<bool>(() => Info.IsNestedAssembly);
		private static readonly Lazy<bool> IsNestedFamANDAssemLazy = new Lazy<bool>(() => Info.IsNestedFamANDAssem);
		private static readonly Lazy<bool> IsNestedFamilyLazy = new Lazy<bool>(() => Info.IsNestedFamily);
		private static readonly Lazy<bool> IsNestedFamORAssemLazy = new Lazy<bool>(() => Info.IsNestedFamORAssem);
		private static readonly Lazy<bool> IsNestedPrivateLazy = new Lazy<bool>(() => Info.IsNestedPrivate);
		private static readonly Lazy<bool> IsNestedPublicLazy = new Lazy<bool>(() => Info.IsNestedPublic);
		private static readonly Lazy<bool> IsNotPublicLazy = new Lazy<bool>(() => Info.IsNotPublic);
		private static readonly Lazy<bool> IsPointerLazy = new Lazy<bool>(() => Info.IsPointer);
		private static readonly Lazy<bool> IsPrimitiveLazy = new Lazy<bool>(() => Info.IsPrimitive);
		private static readonly Lazy<bool> IsPublicLazy = new Lazy<bool>(() => Info.IsPublic);
		private static readonly Lazy<bool> IsSealedLazy = new Lazy<bool>(() => Info.IsSealed);
		private static readonly Lazy<bool> IsSpecialNameLazy = new Lazy<bool>(() => Info.IsSpecialName);
		private static readonly Lazy<bool> IsValueTypeLazy = new Lazy<bool>(() => Info.IsValueType);
		private static readonly Lazy<bool> IsVisibleLazy = new Lazy<bool>(() => Info.IsVisible);
		
		public static bool HasElementType  { get; } = HasElementTypeLazy.Value;
		public static bool IsArray  { get; } = IsArrayLazy.Value;
		public static bool IsAbstract  { get; } = IsAbstractLazy.Value;
		public static bool IsByRef  { get; } = IsByRefLazy.Value;
		public static bool IsClass  { get; } = IsClassLazy.Value;
		public static bool IsEnum  { get; } = IsEnumLazy.Value;
		public static bool IsGenericParameter  { get; } = IsGenericParameterLazy.Value;
		public static bool IsGenericType  { get; } = IsGenericTypeLazy.Value;
		public static bool IsGenericTypeDefinition  { get; } = IsGenericTypeDefinitionLazy.Value;
		public static bool IsInterface  { get; } = IsInterfaceLazy.Value;
		public static bool IsNested  { get; } = IsNestedLazy.Value;
		public static bool IsNestedAssembly  { get; } = IsNestedAssemblyLazy.Value;
		public static bool IsNestedFamANDAssem  { get; } = IsNestedFamANDAssemLazy.Value;
		public static bool IsNestedFamily  { get; } = IsNestedFamilyLazy.Value;
		public static bool IsNestedFamORAssem { get; } = IsNestedFamORAssemLazy.Value;
		public static bool IsNestedPrivate  { get; } = IsNestedPrivateLazy.Value;
		public static bool IsNestedPublic  { get; } = IsNestedPublicLazy.Value;
		public static bool IsNotPublic  { get; } = IsNotPublicLazy.Value;
		public static bool IsPointer  { get; } = IsPointerLazy.Value;
		public static bool IsPrimitive  { get; } = IsPrimitiveLazy.Value;
		public static bool IsPublic { get; } = IsPublicLazy.Value;
		public static bool IsSealed { get; } = IsSealedLazy.Value;
		public static bool IsSpecialName  { get; } = IsSpecialNameLazy.Value;
		public static bool IsValueType { get; } = IsValueTypeLazy.Value;
		public static bool IsVisible  { get; } = IsVisibleLazy.Value;

		public static bool IsAssignableFrom(Type type) => Info.IsAssignableFrom(type);
		
#if CoreCLR
		static TypeOf() { } // see: https://github.com/dotnet/coreclr/issues/17981
#endif
	}
}