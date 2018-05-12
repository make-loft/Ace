using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Ace
{
	// performance optimized
	// ReSharper disable StaticMemberInGenericType
	public static class TypeOf<T>
	{
		public static readonly Type TypeInfo = typeof(T);

		private static string _name, _fullName, _assemblyQualifiedName;
		private static Assembly _assembly;

		public static string Name { get; } = _name ?? TypeInfo.Name.To(out _name);
		public static string FullName { get; } = _fullName ?? TypeInfo.FullName.To(out _fullName);
		public static Assembly Assembly { get; } = _assembly ?? TypeInfo.Assembly.To(out _assembly);
		public static string AssemblyQualifiedName { get; } = _assemblyQualifiedName ?? TypeInfo.AssemblyQualifiedName.To(out _assemblyQualifiedName);

		private static readonly Lazy<bool> HasElementTypeLazy = new Lazy<bool>(() => TypeInfo.HasElementType);
		private static readonly Lazy<bool> IsArrayLazy = new Lazy<bool>(() => TypeInfo.IsArray);
		private static readonly Lazy<bool> IsAbstractLazy = new Lazy<bool>(() => TypeInfo.IsAbstract);
		private static readonly Lazy<bool> IsByRefLazy = new Lazy<bool>(() => TypeInfo.IsByRef);
		private static readonly Lazy<bool> IsClassLazy = new Lazy<bool>(() => TypeInfo.IsClass);
		private static readonly Lazy<bool> IsEnumLazy = new Lazy<bool>(() => TypeInfo.IsEnum);
		private static readonly Lazy<bool> IsGenericParameterLazy = new Lazy<bool>(() => TypeInfo.IsGenericParameter);
		private static readonly Lazy<bool> IsGenericTypeLazy = new Lazy<bool>(() => TypeInfo.IsGenericType);
		private static readonly Lazy<bool> IsGenericTypeDefinitionLazy = new Lazy<bool>(() => TypeInfo.IsGenericTypeDefinition);
		private static readonly Lazy<bool> IsInterfaceLazy = new Lazy<bool>(() => TypeInfo.IsInterface);
		private static readonly Lazy<bool> IsNestedLazy = new Lazy<bool>(() => TypeInfo.IsNested);
		private static readonly Lazy<bool> IsNestedAssemblyLazy = new Lazy<bool>(() => TypeInfo.IsNestedAssembly);
		private static readonly Lazy<bool> IsNestedFamANDAssemLazy = new Lazy<bool>(() => TypeInfo.IsNestedFamANDAssem);
		private static readonly Lazy<bool> IsNestedFamilyLazy = new Lazy<bool>(() => TypeInfo.IsNestedFamily);
		private static readonly Lazy<bool> IsNestedFamORAssemLazy = new Lazy<bool>(() => TypeInfo.IsNestedFamORAssem);
		private static readonly Lazy<bool> IsNestedPrivateLazy = new Lazy<bool>(() => TypeInfo.IsNestedPrivate);
		private static readonly Lazy<bool> IsNestedPublicLazy = new Lazy<bool>(() => TypeInfo.IsNestedPublic);
		private static readonly Lazy<bool> IsNotPublicLazy = new Lazy<bool>(() => TypeInfo.IsNotPublic);
		private static readonly Lazy<bool> IsPointerLazy = new Lazy<bool>(() => TypeInfo.IsPointer);
		private static readonly Lazy<bool> IsPrimitiveLazy = new Lazy<bool>(() => TypeInfo.IsPrimitive);
		private static readonly Lazy<bool> IsPublicLazy = new Lazy<bool>(() => TypeInfo.IsPublic);
		private static readonly Lazy<bool> IsSealedLazy = new Lazy<bool>(() => TypeInfo.IsSealed);
		private static readonly Lazy<bool> IsSpecialNameLazy = new Lazy<bool>(() => TypeInfo.IsSpecialName);
		private static readonly Lazy<bool> IsValueTypeLazy = new Lazy<bool>(() => TypeInfo.IsValueType);
		private static readonly Lazy<bool> IsVisibleLazy = new Lazy<bool>(() => TypeInfo.IsVisible);
		
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
	}
}